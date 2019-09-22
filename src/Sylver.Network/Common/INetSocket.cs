﻿using System;
using System.Net;
using System.Net.Sockets;

namespace Sylver.Network.Common
{
    /// <summary>
    /// Provides an interface to manage a Sylver socket.
    /// </summary>
    public interface INetSocket : IDisposable
    {
        /// <summary>
        /// Gets the underlaying socket instance.
        /// </summary>
        /// <returns>.NET Socket.</returns>
        Socket GetSocket();

        /// <summary>
        /// Places a <see cref="Socket"/> in a listening state.
        /// </summary>
        /// <param name="backlog">The maximum length of the pending connections queue.</param>
        void Listen(int backlog);

        /// <summary>
        /// Associates a System.Net.Sockets.Socket with a local endpoint.
        /// </summary>
        /// <param name="localEP">The local System.Net.EndPoint to associate with the <see cref="Socket"/>.</param>
        void Bind(EndPoint localEP);

        /// <summary>
        /// Sets the specified System.Net.Sockets.Socket option to the specified integer value.
        /// </summary>
        /// <param name="optionLevel">One of the <see cref="SocketOptionLevel"/> values.</param>
        /// <param name="optionName">One of the <see cref="SocketOptionName"/> values.</param>
        /// <param name="optionValue">A value of the option.</param>
        void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue);

        /// <summary>
        /// Begins an asynchronous operation to accept an incoming connection attempt.
        /// </summary>
        /// <param name="socketAsyncEvent"></param>
        /// <returns></returns>
        bool AcceptAsync(SocketAsyncEventArgs socketAsyncEvent);

        /// <summary>
        /// Begins an asynchronous request to receive data from a connected <see cref="Socket"/> object.
        /// </summary>
        /// <param name="socketAsyncEvent"></param>
        /// <returns></returns>
        bool ReceiveAsync(SocketAsyncEventArgs socketAsyncEvent);
    }
}
