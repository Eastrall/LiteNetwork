using LiteNetwork.Protocol.Abstractions;
using System;
using System.Linq;

namespace LiteNetwork.Protocol
{
    /// <summary>
    /// Default LiteNetwork packet processor.
    /// </summary>
    public class LitePacketProcessor : ILitePacketProcessor
    {
        public LitePacketProcessor()
        {
            HeaderSize = sizeof(int);
        }

        public LitePacketProcessor(int headerSize)
        {
            HeaderSize = headerSize;
        }

        public virtual int HeaderSize { get; protected set; }

        public virtual bool IncludeHeader { get; protected set; }

        public virtual int GetMessageLength(byte[] buffer)
        {
            if (buffer.Length < sizeof(int))
                Array.Resize(ref buffer, sizeof(int));

            return BitConverter.ToInt32(BitConverter.IsLittleEndian
                ? buffer.Take(sizeof(int)).ToArray()
                : buffer.Take(sizeof(int)).Reverse().ToArray(), 0);
        }

        public virtual ILitePacketStream CreatePacket(byte[] buffer) => new LitePacket(buffer);

        public virtual bool ParseHeader(LiteDataToken token, byte[] buffer, int bytesTransfered)
        {
            if (token.HeaderData is null)
            {
                token.HeaderData = new byte[HeaderSize];
            }

            int bufferRemainingBytes = bytesTransfered - token.DataStartOffset;

            if (bufferRemainingBytes > 0)
            {
                int headerRemainingBytes = HeaderSize - token.ReceivedHeaderBytesCount;
                int bytesToRead = Math.Min(bufferRemainingBytes, headerRemainingBytes);

                Buffer.BlockCopy(buffer, token.DataStartOffset, token.HeaderData, token.ReceivedHeaderBytesCount, bytesToRead);
                
                token.ReceivedHeaderBytesCount += bytesToRead;
                token.DataStartOffset += bytesToRead;
            }
            
            return token.ReceivedHeaderBytesCount == HeaderSize;
        }

        /// <inheritdoc />
        public virtual void ParseContent(LiteDataToken token, byte[] buffer, int bytesTransfered)
        {
            if (token.HeaderData is null)
            {
                throw new ArgumentException($"Header data is null.");
            }

            if (!token.MessageSize.HasValue)
            {
                token.MessageSize = GetMessageLength(token.HeaderData);
            }

            if (token.MessageSize.Value < 0)
            {
                throw new InvalidOperationException("Message size cannot be smaller than zero.");
            }

            if (token.MessageData is null)
            {
                token.MessageData = new byte[token.MessageSize.Value];
            }

            if (token.ReceivedMessageBytesCount < token.MessageSize.Value)
            {
                int bufferRemainingBytes = bytesTransfered - token.DataStartOffset;
                int messageRemainingBytes = token.MessageSize.Value - token.ReceivedMessageBytesCount;
                int bytesToRead = Math.Min(bufferRemainingBytes, messageRemainingBytes);

                Buffer.BlockCopy(buffer, token.DataStartOffset, token.MessageData, token.ReceivedMessageBytesCount, bytesToRead);

                token.ReceivedMessageBytesCount += bytesToRead;
                token.DataStartOffset += bytesToRead;
            }
        }
    }
}
