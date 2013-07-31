using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using Raveon.Messaging;

namespace Raveon.FileTransferDemo {
    class SerialHandler {
        SerialPort serialPort;

        StringBuilder serialBuffer;

        bool shouldRun;

        public delegate void WMXReceivedMessageDelegate(WMXMessage newMessage);
        public WMXReceivedMessageDelegate WMXReceived { get; set; }

        public SerialHandler(string portName){
            serialPort = new SerialPort(portName, 38400, Parity.None, 8);
            serialPort.Encoding = Encoding.GetEncoding(28591);

            serialBuffer = new StringBuilder();

            shouldRun = false;
        }

        public void sendWMX(WMXMessage wmxMessage) {
            if (shouldRun) {
                serialPort.Write(wmxMessage.Bytes, 0, wmxMessage.ByteCount);
            }
        }

        public void stop() {
            shouldRun = false;
        }

        public void run(){
            serialPort.Open();

            byte[] byteBuffer = new byte[1];

            shouldRun = true; // Set to false using stop()
            try{
                while (shouldRun) {
                    try {
                        serialPort.ReadTimeout = 50;
                        while (serialPort.Read(byteBuffer, 0, 1) > 0) {
                            serialBuffer.Append(Encoding.GetEncoding(28591).GetString(byteBuffer));
                        }
                    } catch (TimeoutException) {
                        // Should happen pretty often!
                    }

                    bool doneParsing = false;
                    while (serialBuffer.Length > 0 && !doneParsing) {
                        MessageParseResult result = WMXMessage.parse(Encoding.GetEncoding(28591).GetBytes(serialBuffer.ToString()));
                        switch (result.Result) {
                        case MessageParseResult.ParseResult.Invalid:
                            // No WMX message starting with this char. Remove the first char from the buffer
                            serialBuffer.Remove(0, 1);
                            break;
                        case MessageParseResult.ParseResult.ValidWithMoreData:
                            if (serialBuffer.Length > 1000) {
                                // Something has gone wrong. Flush the buffer
                                serialBuffer.Clear();
                            }
                            // Either way, we can't parse any more without more data
                            doneParsing = true;
                            break;
                        case MessageParseResult.ParseResult.Complete:
                            // We got a valid WMX message in. Scan through the messages
                            // we've sent out and see if any are relevant to use
                            serialBuffer.Remove(0, result.BytesUsed);

                            WMXMessage message = result.ParsedMessage as WMXMessage;

                            if (WMXReceived != null) {
                                WMXReceived(message);
                            }

                            break;
                        default:
                            break;
                        }
                    }
                }
            }finally{
                shouldRun = false;
                serialPort.Close();
            }
        }
    }
}
