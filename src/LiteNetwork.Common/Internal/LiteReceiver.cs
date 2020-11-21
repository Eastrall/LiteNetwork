﻿using LiteNetwork.Common.Exceptions;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Protocol.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiteNetwork.Common.Internal
{
    internal abstract class LiteReceiver
    {
        private readonly ILitePacketProcessor _packetProcessor;
        private readonly LitePacketParser _packetParser;

        public event EventHandler<ILiteConnection> Disconnected;
        public event EventHandler<Exception> Error;

        protected LiteReceiver(ILitePacketProcessor packetProcessor)
        {
            _packetProcessor = packetProcessor;
            _packetParser = new LitePacketParser(_packetProcessor);
        }

        public void StartReceiving(ILiteConnection connection)
        {

        }

        /// <summary>
        /// Receive data from a client.
        /// </summary>
        /// <param name="userConnectionToken">user connection token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void ReceiveData(ILiteConnectionToken userConnectionToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (userConnectionToken.Socket == null)
            {
                ClearSocketEvent(socketAsyncEvent);
                OnDisconnected(userConnectionToken.Connection);
            }
            else if (!userConnectionToken.Socket.ReceiveAsync(socketAsyncEvent))
            {
                ProcessReceive(userConnectionToken, socketAsyncEvent);
            }
        }

        /// <summary>
        /// Process the received data.
        /// </summary>
        /// <param name="clientToken">Client token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        [ExcludeFromCodeCoverage]
        private void ProcessReceive(ILiteConnectionToken clientToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            try
            {
                if (socketAsyncEvent is null)
                {
                    throw new ArgumentNullException(nameof(socketAsyncEvent), "Cannot receive data from a null socket event.");
                }

                if (socketAsyncEvent.BytesTransferred > 0)
                {
                    if (socketAsyncEvent.SocketError == SocketError.Success)
                    {
                        IEnumerable<byte[]> messages = _packetParser.ParseIncomingData(clientToken.DataToken, socketAsyncEvent.Buffer, socketAsyncEvent.BytesTransferred);

                        if (messages.Any())
                        {
                            foreach (var message in messages)
                            {
                                ProcessReceivedMessage(clientToken, message);
                            }
                        }

                        if (clientToken.DataToken.DataStartOffset >= socketAsyncEvent.BytesTransferred)
                        {
                            clientToken.DataToken.Reset();
                        }

                        ReceiveData(clientToken, socketAsyncEvent);
                    }
                    else
                    {
                        throw new LiteReceiverException(clientToken.Connection, socketAsyncEvent.SocketError);
                    }
                }
                else
                {
                    ClearSocketEvent(socketAsyncEvent);
                    OnDisconnected(clientToken.Connection);
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        /// <summary>
        /// Fired when a receive operation has completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        [ExcludeFromCodeCoverage]
        protected internal void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (sender is null)
                {
                    throw new ArgumentNullException(nameof(sender));
                }

                if (e.UserToken is not ILiteConnectionToken connectionToken)
                {
                    throw new ArgumentException("Incorrect token type.");
                }

                if (e.LastOperation == SocketAsyncOperation.Receive)
                {
                    ProcessReceive(connectionToken, e);
                }
                else
                {
                    throw new InvalidOperationException($"Unknown '{e.LastOperation}' socket operation in receiver.");
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        /// <summary>
        /// Gets a <see cref="SocketAsyncEventArgs"/> for the receive operation.
        /// </summary>
        /// <returns></returns>
        protected abstract SocketAsyncEventArgs GetSocketEvent();

        /// <summary>
        /// Clears an used <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async vent arguments to clear.</param>
        protected abstract void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent);

        /// <summary>
        /// Client has been disconnected.
        /// </summary>
        /// <param name="client">Disconnected client.</param>
        private void OnDisconnected(ILiteConnection client) => Disconnected?.Invoke(this, client);

        /// <summary>
        /// Called when an exeption has been thrown during the receive process.
        /// </summary>
        /// <param name="exception">Thrown exception.</param>
        private void OnError(Exception exception) => Error?.Invoke(this, exception);

        /// <summary>
        /// Process a received message.
        /// </summary>
        /// <param name="client">Current client.</param>
        /// <param name="messageBuffer">Current message data buffer.</param>
        [ExcludeFromCodeCoverage]
        protected virtual void ProcessReceivedMessage(ILiteConnectionToken connectionToken, byte[] messageBuffer)
        {
            Task.Run(() =>
            {
                try
                {
                    // Create stream.
                    connectionToken.Connection.HandleMessageAsync(null);
                }
                catch (Exception e)
                {
                    OnError(e);
                }
            });
        }
    }
}
