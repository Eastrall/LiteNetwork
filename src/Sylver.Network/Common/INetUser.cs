﻿using Sylver.Network.Data;

namespace Sylver.Network.Common
{
    public interface INetUser : INetConnection
    {
        /// <summary>
        /// Handle an incoming packet.
        /// </summary>
        /// <param name="packet">Incoming packet.</param>
        void HandleMessage(INetPacketStream packet);
    }
}
