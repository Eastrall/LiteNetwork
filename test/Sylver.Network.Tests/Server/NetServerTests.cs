﻿using Moq;
using Sylver.Network.Common;
using Sylver.Network.Server;
using Sylver.Network.Tests.Mocks;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetServerTests
    {
        private readonly SocketMock _socketMock;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly Mock<NetServer<CustomClient>> _server;

        public NetServerTests()
        {
            this._socketMock = new SocketMock(true, true);
            this._serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444, 50, 100, 128);
            this._server = new Mock<NetServer<CustomClient>>(this._serverConfiguration);
            this._server.SetupGet(x => x.Socket).Returns(this._socketMock);
        }

        [Fact]
        public void StartServerTest()
        {
            Assert.False(this._server.Object.IsRunning);

            this._server.Object.Start();

            Assert.True(this._server.Object.IsRunning);
            this._socketMock.VerifySetSocketOptions(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this._socketMock.VerifyBind(NetHelper.CreateIpEndPoint(this._serverConfiguration.Host, this._serverConfiguration.Port));
            this._socketMock.VerifyListen(this._serverConfiguration.Backlog);
        }

        [Fact]
        public void StopServerTest()
        {
            Assert.False(this._server.Object.IsRunning);
            this._server.Object.Start();
            Assert.True(this._server.Object.IsRunning);
            this._server.Object.Stop();
            Assert.False(this._server.Object.IsRunning);
            this._socketMock.VerifyDispose();
        }
    }
}
