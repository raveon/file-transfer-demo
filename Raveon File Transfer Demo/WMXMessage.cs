using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace Raveon.Messaging
{
	public class WMXMessage : Message
	{
		public enum MessageType {
			TxData = 0x40,
			RxData = 0x42,
            OTAAck = 0x43,
			Command = 0x44,
			CommandResponse = 0x45,
            MessageStatus = 0x47
		};

		/// <summary>
		/// Bytes that require special byte stuffing behavior
		/// </summary>
		static List<byte> stuffBytes = new List<byte>(new byte[] {0xFF, 0x03, 0x04, 0x0D});

        [Browsable(false)]
		public byte[] Bytes {
			get { return fullMessage; }
			private set { } 
		}

        [DisplayName("From")]
        public int FromID { get; set; }

        [DisplayName("To")]
        public int ToID { get; set; }

        public int Sequence { get; set; }

		public string ASCIIPayload {
			get { return Encoding.GetEncoding(28591).GetString (Data); }
			private set { }
		}

        [DisplayName("Total Bytes")]
        public int ByteCount {
            get { return fullMessage.Length; }
            private set { }
        }

		public MessageType Type {get; set; }

        [Browsable(false)]
		public byte[] Data;
		private byte[] fullMessage;

        public int StatusSequence { get; private set; }
        public char StatusValue { get; private set; }

		/// <summary>
		/// Unique sequence numbers are extremely important. They can range from 0-255
		/// </summary>
		private static int nextSequence = 0;

		public WMXMessage (int fromID, int toID, MessageType type, string message)
		: this(fromID, toID, type, Encoding.GetEncoding(28591).GetBytes(message))
		{

		}

		public WMXMessage (int fromID, int toID, MessageType type, byte[] data)
		{
			this.FromID = fromID;
			this.ToID = toID;
			this.Type = type;
			this.Data = data;

			List<byte> message = new List<byte> ();
			// SOH
			message.Add (0x01);
			// Control Field
			message.Add((byte) type);
			// TOID
			message.AddRange (Encoding.GetEncoding(28591).GetBytes (toID.ToString ("X4")));
			// Separator 1 (!)
			message.Add (0x21);
			// From ID
			message.AddRange (Encoding.GetEncoding(28591).GetBytes (fromID.ToString ("X4")));
			// Separator 3 (#)
			message.Add (0x23);
			// Sequence number
			message.AddRange (Encoding.GetEncoding(28591).GetBytes (nextSequence.ToString()));

            Sequence = nextSequence;
			nextSequence++; // Should have a mutex, really.
			if (nextSequence > 255) {
				nextSequence = 0;
			}

			// SOT
			message.Add(0x02);
			// Data bytes, with byte stuffing
			foreach (byte datum in data) {
				if(stuffBytes.Contains(datum)){
					// Stuff this byte
					message.Add(0xFF);
					message.Add((byte) (0xFF - datum));
				}else{
					// No need to stuff
					message.Add(datum);
				}
			}
			// End of Text (DLE ETX)
			message.Add(0x10);
			message.Add(0x03);
			// EOT
			message.Add(0x04);

			// Store the binary message
			fullMessage = message.ToArray();

            // Store some meta-information for certain messages
            StatusSequence = -1;
            StatusValue = '\0';

            if (Type == MessageType.MessageStatus) {
                // This should have a body in the form <Status Char>,<Sequence>,<Original TOID>
                string[] messageParts = ASCIIPayload.Split(',');
                if (messageParts.Length > 2 && messageParts[0].Length == 1) {
                    int statusSequence = 0;
                    if (int.TryParse(messageParts[1], out statusSequence)) {
                        StatusSequence = statusSequence;
                        StatusValue = messageParts[0][0];
                    }
                }
            } else if (Type == MessageType.OTAAck) {
                // This should have a body that contains the original sequence number being ACKed
                int statusSequence = 0;
                if(int.TryParse(ASCIIPayload, out statusSequence)){
                    StatusSequence = statusSequence;
                }
            }
		}

		/// <summary>
		/// Attempt to pull a single WMX message out of the given buffer. The WMX
		/// message will only be pulled if it starts at the very first byte
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public static MessageParseResult parse (byte[] parseBuffer)
		{
			// Init the result to invalid
			MessageParseResult result = new MessageParseResult ();
			result.ParsedType = typeof(WMXMessage);
			result.BytesUsed = 0;
			result.ParsedMessage = null;
			result.Result = MessageParseResult.ParseResult.Invalid;

			if (parseBuffer [0] != 0x01) {
				// No SOH
				return result;
			}

			if (parseBuffer.Length < 2) {
				// Just got an SOH. Wait for more data.
				result.Result = MessageParseResult.ParseResult.ValidWithMoreData;
				return result;
			}

			if (!Enum.IsDefined (typeof(MessageType), (int) parseBuffer [1])) {
				// Invalid control byte
				return result;
			}
			MessageType messageType = (MessageType)parseBuffer [1];

			// Starting to look good. Look for all the key components
			int separator1Index = Array.IndexOf (parseBuffer, (byte) 0x21);
			int sotIndex = Array.IndexOf (parseBuffer, (byte) 0x02);

			// See if we can get a To ID
			if (separator1Index < -1) {
				if (parseBuffer.Length < 7) {
					// Not enough data yet
					result.Result = MessageParseResult.ParseResult.ValidWithMoreData;
				}
				return result;
			} else if (separator1Index < 3 || separator1Index > 10) { // Really the max index is 6, but ASCII messages cause Cigorn to send to FFFFFFFF
				// Too early or too late
				return result;
			}
			int toId = -1;
			if (!int.TryParse (Encoding.GetEncoding(28591).GetString (parseBuffer, 2, separator1Index - 2), NumberStyles.HexNumber, null as IFormatProvider, out toId)) {
				// Invalid character or something
				return result;
			}

			// TODO: For now, just jump to the SOT. Sometime soon add in Source, Group, Sequence and RSSI parsing
			int fromId = 0;

			// Make sure we can start the message
			if (sotIndex < 0) {
				if (parseBuffer.Length <= separator1Index + 31) {
					// Not enough data yet
					result.Result = MessageParseResult.ParseResult.ValidWithMoreData;
				}
				return result;
			} else if (sotIndex - separator1Index > 31 + 1) {
				// Too far
				return result;
			}

			// TODO: We repeat this same pattern a lot. Should be a function
			// Look for the end of the message
			int etxIndex = Array.IndexOf (parseBuffer, (byte) 0x03, sotIndex);
			if (etxIndex < 0) {
				if (parseBuffer.Length <= sotIndex + 500) {
					// It could still come...
					result.Result = MessageParseResult.ParseResult.ValidWithMoreData;
				}
				return result;
			} else if (etxIndex - sotIndex > 500 + 1) {
				// Too long
				return result;
			} else if (parseBuffer [etxIndex - 1] != 0x10) {
				// It's supposed to be DLE ETX. Anything else is invalid
				return result;
			}

			// Ignore the checksum, but make sure we have the EOT
			int eotIndex = Array.IndexOf (parseBuffer, (byte) 0x04, etxIndex);
			if (eotIndex < 0) {
				if (parseBuffer.Length <= etxIndex + 4) {
					// Not enough data yet
					result.Result = MessageParseResult.ParseResult.ValidWithMoreData;
				}
				return result;
			} else if (eotIndex - etxIndex > 4 + 1) {
				// Too far
				return result;
			}

			// Alright, we're going to accept this.
			result.Result = MessageParseResult.ParseResult.Complete;
			result.BytesUsed = eotIndex + 1;

			// Because of the byte stuffing, we have to go through every byte
			List<byte> dataBuffer = new List<byte>();
			for (int i = sotIndex + 1; i < etxIndex - 1; i++) {
				if(parseBuffer[i] == 0xFF){
					// Stuffed byte
					dataBuffer.Add((byte) (0xFF - parseBuffer[i+1])); 

					// Skip next
					i++;
				}else{
					dataBuffer.Add (parseBuffer[i]);
				}
			}
			result.ParsedMessage = new WMXMessage(fromId, toId, messageType, dataBuffer.ToArray());

			return result;
		}
	}
}

