using LiteNetwork.Protocol;
using LiteNetwork.Protocol.Abstractions;
using System;

namespace LiteNetwork.Client
{
    /// <summary>
    /// Provides a data structure that describes the client options.
    /// </summary>
    public class LiteClientOptions
    {
        /// <summary>
        /// Gets the default buffer allocated size.
        /// </summary>
        public const int DefaultBufferSize = 128;

        /// <summary>
        /// Gets the default header size, which is 4 bytes (int value).
        /// </summary>
        public const int DefaultHeaderSize = 4;

        /// <summary>
        /// Gets or sets the remote host to connect.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the remote port to connect.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the handled client buffer size.
        /// </summary>
        public int BufferSize { get; set; } = DefaultBufferSize;

        /// <summary>
        /// TCP packet header size in bytes.
        /// </summary>
        public int HeaderSize { get; set; } = DefaultHeaderSize;

        /// <summary>
        /// Gets or sets the receive strategy type.
        /// </summary>
        public ReceiveStrategyType ReceiveStrategy { get; set; }

        private readonly Lazy<ILitePacketProcessor> _lazyPacketProcessor;
        /// <summary>
        /// Gets the default server packet processor.
        /// </summary>
        public ILitePacketProcessor PacketProcessor { get => _lazyPacketProcessor.Value; }

        /// <summary>
        /// Creates and initializes a new <see cref="LiteClientOptions"/> instance
        /// with a default <see cref="LitePacketProcessor"/>.
        /// </summary>
        public LiteClientOptions()
        {
            _lazyPacketProcessor = new Lazy<ILitePacketProcessor>(() => new LitePacketProcessor(HeaderSize));
        }
    }
}
