﻿using System;
using System.IO;
using System.Text;

namespace Managed.Adb
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ChunkHandler
	{
		/// <summary>
		/// 
		/// </summary>
		public enum ByteOrder
		{
			/// <summary>
			/// 
			/// </summary>
			LittleEndian,
			/// <summary>
			/// 
			/// </summary>
			BigEndian
		}

		/// <summary>
		/// 
		/// </summary>
		public const int CHUNK_HEADER_LEN = 8;   // 4-byte type, 4-byte len
		/// <summary>
		/// 
		/// </summary>
		public const ByteOrder CHUNK_ORDER = ByteOrder.BigEndian;

		/// <summary>
		/// 
		/// </summary>
		public const int CHUNK_FAIL = -1;

		/// <summary>
		/// Prevents a default instance of the <see cref="ChunkHandler"/> class from being created.
		/// </summary>
		ChunkHandler() { }

		/**
		 * Client is ready.  The monitor thread calls this method on all
		 * handlers when the client is determined to be DDM-aware (usually
		 * after receiving a HELO response.)
		 *
		 * The handler can use this opportunity to initialize client-side
		 * activity.  Because there's a fair chance we'll want to send a
		 * message to the client, this method can throw an IOException.
		 */
		public abstract void ClientReady(IClient client);

		/**
		 * Client has gone away.  Can be used to clean up any resources
		 * associated with this client connection.
		 */
		public abstract void ClientDisconnected(IClient client);

		/**
		 * Handle an incoming chunk.  The data, of chunk type "type", begins
		 * at the start of "data" and continues to data.limit().
		 *
		 * If "isReply" is set, then "msgId" will be the ID of the request
		 * we sent to the client.  Otherwise, it's the ID generated by the
		 * client for this event.  Note that it's possible to receive chunks
		 * in reply packets for which we are not registered.
		 *
		 * The handler may not modify the contents of "data".
		 */
		public abstract void HandleChunk(IClient client, int type,
						byte[] data, bool isReply, int msgId);

		/**
		 * Handle chunks not recognized by handlers.  The handleChunk() method
		 * in sub-classes should call this if the chunk type isn't recognized.
		 */
		protected void HandleUnknownChunk(IClient client, int type,
						MemoryStream data, bool isReply, int msgId)
		{
			if (type == CHUNK_FAIL)
			{
				int errorCode, msgLen;
				string msg;

				errorCode = data.ReadByte();
				msgLen = data.ReadByte();

				msg = GetString(data, msgLen);
				Log.W("ddms", "WARNING: failure code=" + errorCode + " msg=" + msg);
			}
			else
			{
				Log.W("ddms", "WARNING: received unknown chunk " + Name(type)
								+ ": len=" + data.Length + ", reply=" + isReply
								+ ", msgId=0x" + msgId.ToString("X8"));
			}
			Log.W("ddms", "         client " + client + ", handler " + this);
		}


		/**
		 * Utility function to copy a string out of a ByteBuffer.
		 *
		 * This is here because multiple chunk handlers can make use of it,
		 * and there's nowhere better to put it.
		 */
		static string GetString(MemoryStream buf, int len)
		{
			char[] data = new char[len];
			for (int i = 0; i < len; i++)
				data[i] = (char)buf.ReadByte();
			return new string(data);
		}

		/**
		 * Convert an integer type to a 4-character string.
		 */
		static string Name(int type)
		{
			char[] ascii = new char[4];

			ascii[0] = (char)((type >> 24) & 0xff);
			ascii[1] = (char)((type >> 16) & 0xff);
			ascii[2] = (char)((type >> 8) & 0xff);
			ascii[3] = (char)(type & 0xff);

			return new string(ascii);
		}
	}
}
