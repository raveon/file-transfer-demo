using System;

namespace Raveon.Messaging
{
	/// <summary>
	/// Provides the result of an attempt to parse a message
	/// </summary>
	public class MessageParseResult
	{
		public enum ParseResult{
			/// <summary>
			/// Indicates 
			/// </summary>
			Invalid,
			ValidWithMoreData,
			Complete
		}
		public ParseResult Result {get; set;}
		public int BytesUsed {get; set;}
		public Message ParsedMessage {get; set;}
		public Type ParsedType {get; set;}

		public MessageParseResult(){
			Result = ParseResult.Invalid;
		}
	}
}
