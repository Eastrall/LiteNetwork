using System;
using System.IO;

namespace LiteNetwork.Protocol
{
    /// <summary>
    /// Provides a default packet.
    /// </summary>
    public class LitePacket : LitePacketStream
    {
        /// <summary>
        /// Gets the default <see cref="LitePacket"/> header size. (4 bytes)
        /// </summary>
        public readonly int HeaderSize = sizeof(int);

        /// <inheritdoc />
        public override byte[] Buffer
        {
            get
            {
                if (Mode == LitePacketMode.Write)
                {
                    long oldPosition = Position;

                    Seek(0, SeekOrigin.Begin);

                    var headerBytes = BitConverter.GetBytes(ContentLength); // in little endian
                    if (!BitConverter.IsLittleEndian)
                        Array.Reverse(headerBytes);

                    // Take only header size, not bigger!
                    for (var i = 0; i < HeaderSize; i++)
                    {
                        Write(headerBytes[i]);
                    }

                    Seek((int)oldPosition, SeekOrigin.Begin);
                }

                return base.Buffer;
            }
        }

        /// <summary>
        /// Gets the packet's content length.
        /// </summary>
        public long ContentLength => Length - HeaderSize;

        /// <summary>
        /// Creates a new <see cref="LitePacket"/> in write-only mode.
        /// </summary>
        public LitePacket()
        {
            WriteInt32(0); // Packet size (int: 4 bytes)
        }

        /// <summary>
        /// Creates a new <see cref="LitePacket"/> in write-only mode with specific header size (in bytes).
        /// </summary>
        public LitePacket(byte headerSize)
        {
            HeaderSize = headerSize;

            for (var i = 0; i < headerSize; i++)
                WriteByte(0);
        }

        /// <summary>
        /// Creates a new <see cref="LitePacket"/> in read-only mode with an array of bytes.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public LitePacket(byte[] buffer)
            : base(buffer)
        {
        }
    }
}
