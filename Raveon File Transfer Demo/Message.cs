using System;
using System.Collections.Generic;
using System.Reflection;

namespace Raveon.Messaging
{
	/// <summary>
	/// A generic message.
	/// </summary>
	public interface Message
	{
		byte[] Bytes { get; }
        int ByteCount { get; }
	}
}

