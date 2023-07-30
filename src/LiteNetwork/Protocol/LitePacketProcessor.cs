using LiteNetwork.Protocol.Abstractions;
using System;
using System.Buffers.Binary;

namespace LiteNetwork.Protocol;

/// <summary>
/// Default LiteNetwork packet processor.
/// </summary>
public class LitePacketProcessor : ILitePacketProcessor
{
    /// <summary>
    ///     Create a new <see cref="LitePacketProcessor"/>.
    /// </summary>
    public LitePacketProcessor()
    {
        IsLittleEndianMode = BitConverter.IsLittleEndian;
    }

    /// <summary>
    /// Create a new <see cref="LitePacketProcessor"/>.
    /// </summary>
    /// <param name="isLittleEndianMode">Header byte order mode.</param>
    public LitePacketProcessor(bool isLittleEndianMode)
    {
        IsLittleEndianMode = isLittleEndianMode;
    }

    /// <inheritdoc/>
    public virtual int HeaderSize { get; protected set; } = sizeof(int);

    /// <inheritdoc/>
    public virtual bool IncludeHeader { get; protected set; }

    /// <summary>
    /// Header byte order mode.
    /// </summary>
    public bool IsLittleEndianMode { get; }

    /// <inheritdoc/>
    public virtual int GetMessageLength(byte[] buffer)
    {
        return IsLittleEndianMode
            ? BinaryPrimitives.ReadInt32LittleEndian(buffer)
            : BinaryPrimitives.ReadInt32BigEndian(buffer);
    }

    /// <inheritdoc/>
    public virtual bool ReadHeader(LiteDataToken token, byte[] buffer, int bytesTransfered)
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

    /// <inheritdoc/>
    public virtual void ReadContent(LiteDataToken token, byte[] buffer, int bytesTransfered)
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

    /// <inheritdoc/>
    public virtual byte[] AppendHeader(byte[] buffer)
    {
        int contentLength = buffer.Length;
        byte[] packetBuffer = new byte[HeaderSize + buffer.Length];
        
        if (IsLittleEndianMode)
        {
            BinaryPrimitives.WriteInt32LittleEndian(packetBuffer, contentLength);
        }
        else
        {
            BinaryPrimitives.WriteInt32BigEndian(packetBuffer, contentLength);
        }

        Array.Copy(buffer, 0, packetBuffer, HeaderSize, contentLength);

        return packetBuffer;
    }
}
