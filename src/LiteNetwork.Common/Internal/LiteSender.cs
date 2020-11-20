﻿using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LiteNetwork.Common.Internal
{
    internal abstract class LiteSender
    {
        private readonly BlockingCollection<LiteSendingMessage> _sendingCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        private bool _disposedValue;

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates and initializes a new <see cref="NetSender"/> base instance.
        /// </summary>
        protected LiteSender()
        {
            _sendingCollection = new BlockingCollection<LiteSendingMessage>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        /// <inheritdoc />
        public void Start()
        {
            Task.Factory.StartNew(ProcessSendingQueue,
                _cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            IsRunning = true;
        }

        /// <inheritdoc />
        public void Stop()
        {
            _cancellationTokenSource.Cancel(false);
            IsRunning = false;
        }

        /// <inheritdoc />
        public void Send(LiteSendingMessage message) => _sendingCollection.Add(message);

        /// <summary>
        /// Gets a <see cref="SocketAsyncEventArgs"/> for the sending operation.
        /// </summary>
        /// <returns></returns>
        protected abstract SocketAsyncEventArgs GetSocketEvent();

        /// <summary>
        /// Clears an used <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async vent arguments to clear.</param>
        protected abstract void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent);

        /// <summary>
        /// Dequeue the message collection and sends the messages to their recipients.
        /// </summary>
        private void ProcessSendingQueue()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    LiteSendingMessage message = _sendingCollection.Take(_cancellationToken);

                    if (message.Connection is not null && message.Data is not null)
                    {
                        SendMessage(message.Connection, message.Data);
                    }
                }
                catch (OperationCanceledException)
                {
                    // The operation has been cancelled: nothing to do
                }
            }
        }

        /// <summary>
        /// Sends the message data to the given <see cref="INetConnection"/>.
        /// </summary>
        /// <param name="connection">Client connection.</param>
        /// <param name="data">Message data.</param>
        private void SendMessage(Socket connectionSocket, byte[] data)
        {
            SocketAsyncEventArgs socketAsyncEvent = GetSocketEvent();

            socketAsyncEvent.SetBuffer(data, 0, data.Length);

            if (connectionSocket is null)
            {
                ClearSocketEvent(socketAsyncEvent);
                return;
            }

            if (!connectionSocket.SendAsync(socketAsyncEvent))
            {
                OnSendCompleted(this, socketAsyncEvent);
            }
        }

        /// <summary>
        /// Fired when a send operation has been completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        protected void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            ClearSocketEvent(e);
        }

        /// <summary>
        /// Disposes the sender resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    _sendingCollection.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="NetSender"/> resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}