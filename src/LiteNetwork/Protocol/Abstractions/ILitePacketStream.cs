using System;

namespace LiteNetwork.Protocol.Abstractions
{
    /// <summary>
    /// Provides a basic mechanism to read and write inside packet stream.
    /// </summary>
    public interface ILitePacketStream : IDisposable
    {
        /// <summary>
        /// Gets the packet stream buffer.
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// Reads a <typeparamref name="T"/> value from the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value to read.</typeparam>
        /// <returns>The value read and converted to the type.</returns>
        T Read<T>();

        /// <summary>
        /// Writes a <typeparamref name="T"/> value in the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">Value to write in the packet stream.</param>
        void Write<T>(T value);
    }
}
