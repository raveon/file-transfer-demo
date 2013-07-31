using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Raveon.Messaging;

namespace Raveon.FileTransferDemo {
    class FileTransmitHandler {
        public const int FileChunkSize = 128;

        private Mutex inboundWMXMutex;
        private List<WMXMessage> inboundWMX;

        private Mutex outboundWMXMutex;
        private List<WMXMessage> outboundWMX;

        List<MessageStatusTracker> pendingMessages;
        List<IOTAMessage> allMessages;
        string filePath;

        public delegate void WMXMessagesAvailableDelegate();
        public WMXMessagesAvailableDelegate WMXMessagesAvailable { get; set; }

        public FileTransmitHandler(string path) {
            filePath = path;

            inboundWMXMutex = new Mutex();
            inboundWMX = new List<WMXMessage>();

            outboundWMXMutex = new Mutex();
            outboundWMX = new List<WMXMessage>();
        }

        public IEnumerable<WMXMessage> getOutboundMessages() {
            if (outboundWMX.Count == 0) {
                return new WMXMessage[0];
            } else {
                outboundWMXMutex.WaitOne();
                WMXMessage[] messages = outboundWMX.ToArray();
                outboundWMX.Clear();
                outboundWMXMutex.ReleaseMutex();

                return messages;
            }
        }

        public void addInboundMessage(WMXMessage message) {
            inboundWMXMutex.WaitOne();
            try {
                inboundWMX.Add(message);
            } finally {
                inboundWMXMutex.ReleaseMutex();
            }
        }

        public void send(bool requireOTAAcks) {
            pendingMessages = new List<MessageStatusTracker>();
            allMessages = new List<IOTAMessage>();

            string fileName = Path.GetFileName(filePath);

            FileStream fileStream = File.OpenRead(filePath);
            long fileSize = fileStream.Length;
            uint chunks = (uint) Math.Ceiling(fileSize / (double) FileChunkSize);
            uint nextMessageChunk = 0;

            if (requireOTAAcks) {
                //queueOTAMessage(new OTACommand("ATAK", "1"));
            } else {
                //queueOTAMessage(new OTACommand("ATAK", "0"));
            }

            // Track the last time we got a NACK from the local modem
            // We won't send any more data for 1 second after getting one
            // to allow buffers to clear
            Stopwatch lastNACK = new Stopwatch();
            lastNACK.Restart();

            while (pendingMessages.Count > 0 || nextMessageChunk < chunks) {
                // Check any incoming WMX messages that may have come in
                inboundWMXMutex.WaitOne();
                foreach (WMXMessage message in inboundWMX) {
                    foreach (MessageStatusTracker pendingMessage in pendingMessages) {
                        if (pendingMessage.Message.UpdateStatus(message)) {
                            pendingMessage.InteractionTimer.Restart();
                            // The incoming WMX message was consumed
                            break;
                        }
                    }

                    if (message.Type == WMXMessage.MessageType.MessageStatus
                        && message.StatusValue == 'N') {
                        lastNACK.Restart();
                    }
                }
                inboundWMXMutex.ReleaseMutex();

                // We'll only send one message per loop, and only if it's been a 
                // while since we got a NACK. This will allow backing off cleanly
                // when modem buffers are full
                bool sentMessage = false;
                bool allowSend = lastNACK.ElapsedMilliseconds > 1000;
                
                //foreach (MessageStatusTracker messageTracker in pendingMessages) {
                for(int i = pendingMessages.Count - 1; i >= 0; i--){
                    MessageStatusTracker messageTracker = pendingMessages[i];
                    switch (messageTracker.Message.Status) {
                        case OTAMessageStatus.Pending:
                            if (allowSend && !sentMessage) {
                                // Send this message to the modem using WMX
                                WMXMessage wmxMessage = messageTracker.Message.GetMessage(9);

                                outboundWMXMutex.WaitOne();
                                outboundWMX.Add(wmxMessage);
                                outboundWMXMutex.ReleaseMutex();

                                if (WMXMessagesAvailable != null) {
                                    WMXMessagesAvailable();
                                }

                                // It will take a little while for the radio to respond to this. Give it a chance
                                Thread.Sleep(100);

                                messageTracker.Message.Status = OTAMessageStatus.SentToModem;
                                messageTracker.InteractionTimer.Restart();
                                sentMessage = true;
                            }
                            break;

                        case OTAMessageStatus.SentToModem:
                            if (messageTracker.InteractionTimer.ElapsedMilliseconds > 1000) {
                                // We should have gotten an answer back instantly. Send again
                                messageTracker.Message.Status = OTAMessageStatus.Pending;
                            }
                            break;

                        case OTAMessageStatus.Queued:
                            if (messageTracker.InteractionTimer.ElapsedMilliseconds > 10000) {
                                // Been a while. Perhaps it was lost?
                                // If TDMA is on, this might be a normal occurrence and this
                                // should be adjusted accordingly
                                messageTracker.Message.Status = OTAMessageStatus.Pending;
                            }
                            break;

                        case OTAMessageStatus.Transmitted:
                            if (messageTracker.SuccessOn == OTAMessageStatus.Transmitted) {
                                // Done sending this message
                                pendingMessages.Remove(messageTracker);
                            } else {
                                if (messageTracker.InteractionTimer.ElapsedMilliseconds > 2000) {
                                    // Should have gotten the OTA ack by now
                                    messageTracker.Message.Status = OTAMessageStatus.Pending;
                                }
                            }
                            break;

                        case OTAMessageStatus.OTAAcknowledged:
                            // Done sending this message
                            pendingMessages.Remove(messageTracker);
                            break;

                        default:
                            // Shouldn't happen
                            messageTracker.Message.Status = OTAMessageStatus.Pending;
                            break;
                    }
                }

                if (allowSend && !sentMessage && nextMessageChunk < chunks) {
                    // Make sure we don't have any commands queued. Since commands
                    // might be adjusting message flow in some way, we halt transmission
                    // until they complete
                    bool commandQueued = false;
                    foreach(MessageStatusTracker pendingMessage in pendingMessages){
                        if(pendingMessage.Message is OTACommand){
                            commandQueued = true;
                            break;
                        }
                    }
                    if(!commandQueued){
                        // No command queued. No NACKs in a while and we didn't
                        // send anything this time around. Queue a new message

                        // See how much data we're actually going to send
                        int bytesToSend = FileChunkSize;
                        if ((nextMessageChunk + 1) * FileChunkSize > fileSize) {
                            // End of file
                            bytesToSend = (int) (fileSize - (nextMessageChunk * FileChunkSize));
                        }

                        // The actual OTA data is [ASCII filename][null][uint chunk number][data...]
                        byte[] messageData = new byte[fileName.Length + 1 + sizeof(uint) + bytesToSend];
                        
                        // Copy in file name
                        Encoding.ASCII.GetBytes(fileName + '\0').CopyTo(messageData, 0);
                        
                        // Put chunk number in
                        BitConverter.GetBytes(nextMessageChunk).CopyTo(messageData, fileName.Length + 1);

                        // Copy in the data
                        fileStream.Read(messageData, messageData.Length - bytesToSend, bytesToSend);

                        queueOTAMessage(new TransferChunk(messageData));

                        nextMessageChunk++;
                    }
                }
            }
        }

        private void queueOTAMessage(IOTAMessage message) {
            allMessages.Add(message);
            pendingMessages.Add(new MessageStatusTracker(message, OTAMessageStatus.Transmitted));
        }
    }

    class MessageStatusTracker {
        public IOTAMessage Message { get; set; }
        public OTAMessageStatus SuccessOn { get; set; }
        public Stopwatch InteractionTimer { get; set; }

        public MessageStatusTracker(IOTAMessage message, OTAMessageStatus successValue){
            Message = message;
            SuccessOn = successValue;
            InteractionTimer = new Stopwatch();
            InteractionTimer.Reset();
        }
    }

    enum OTAMessageStatus {
        Pending,
        SentToModem,
        Queued,
        Transmitted,
        OTAAcknowledged
    }

    class OTAMessageHelpers {
        public static OTAMessageStatus GetNewStatus(char statusMessageValue, OTAMessageStatus oldStatus) {
            switch (statusMessageValue) {
                case 'Q':
                    return OTAMessageStatus.Queued;
                case 'N':
                    return OTAMessageStatus.Pending;
                case 'L':
                    // This should not happen for this demo- we don't send local messages
                    throw new Exception("Check ID settings!");
                case 'T':
                    return OTAMessageStatus.Transmitted;
                case 'F':
                    return OTAMessageStatus.Pending;
                default:
                    return oldStatus;
            }
        }
    }

    interface IOTAMessage {
        /// <summary>
        /// WMX message which will transfer this message over-the-air when sent to a modem
        /// It's important that a new WMX message is created every time we want to
        /// transmit data to avoid duplicate packet issues
        /// </summary>
        WMXMessage GetMessage(int destinationAddress);

        /// <summary>
        /// Sequence number of the last WMX message created. This sequence number is crucial
        /// in checking whether the OTA message has succeeded
        /// </summary>
        int WMXMessageSequence { get; }

        /// <summary>
        /// Current status of this OTA message
        /// </summary>
        OTAMessageStatus Status { get; set; }

        /// <summary>
        /// Update the status of this OTA message based on the incoming WMX message. The
        /// instance will check if the incoming message pertains to it and update it's
        /// Success status accordingly
        /// </summary>
        /// <param name="message">Incoming WMX message</param>
        /// <returns>True if the WMX message was relevant to this OTA message</returns>
        bool UpdateStatus(WMXMessage message);
    }

    class OTACommand : IOTAMessage {
        string parameter;
        string value;

        public int WMXMessageSequence { get; private set; }
        public OTAMessageStatus Status { get; set; }

        public OTACommand(string parameter, string value) {
            this.parameter = parameter;
            this.value = value;
            this.Status = OTAMessageStatus.Pending;
        }

        public WMXMessage GetMessage(int destinationAddress) {
            WMXMessage message = new WMXMessage(0, destinationAddress, WMXMessage.MessageType.Command, parameter + " " + value);
            WMXMessageSequence = message.Sequence;
            return message;
        }

        public bool UpdateStatus(WMXMessage message) {
            switch (message.Type){
                case WMXMessage.MessageType.CommandResponse:
                    string expectedResponse = parameter.ToUpperInvariant() + "=" + value.ToUpperInvariant();
                    if (message.ASCIIPayload.ToUpperInvariant().Trim() == expectedResponse) {
                        Status = OTAMessageStatus.OTAAcknowledged;
                        return true;
                    }
                    break;
                case WMXMessage.MessageType.MessageStatus:
                    if (message.StatusSequence != WMXMessageSequence) {
                        // Not an update for this instance
                        break;
                    }
                    Status = OTAMessageHelpers.GetNewStatus(message.StatusValue, Status);
                    return true;
            }

            return false;
        }
    }

    class TransferChunk : IOTAMessage {
        byte[] data;

        public int WMXMessageSequence { get; private set; }
        public OTAMessageStatus Status { get; set; }

        public TransferChunk(byte[] chunkData) {
            data = chunkData;
            this.Status = OTAMessageStatus.Pending;
        }

        public WMXMessage GetMessage(int destinationAddress){
            WMXMessage message = new WMXMessage(0, destinationAddress, WMXMessage.MessageType.TxData, data);
            WMXMessageSequence = message.Sequence;
            return message;
        }

        public bool UpdateStatus(WMXMessage message) {
            switch (message.Type) {
                case WMXMessage.MessageType.OTAAck:
                    if (message.StatusSequence != WMXMessageSequence) {
                        // Not an update for this instance
                        break;
                    }
                    Status = OTAMessageStatus.OTAAcknowledged;
                    return true;
                case WMXMessage.MessageType.MessageStatus:
                    if (message.StatusSequence != WMXMessageSequence) {
                        // Not an update for this instance
                        break;
                    }
                    Status = OTAMessageHelpers.GetNewStatus(message.StatusValue, Status);
                    return true;
            }

            return false;
        }
    }
}
