using LiteNetwork.Protocol;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server;
using System;
using System.Threading.Tasks;

namespace LiteNetwork.Sample.Echo.Server
{
    public class ClientUser : LiteServerUser
    {
        public override Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            string receivedMessage = incomingPacketStream.Read<string>();

            Console.WriteLine($"Received from '{Id}': {receivedMessage}");

            using var packet = new LitePacket();
            packet.Write($"Received: '{receivedMessage}'.");

            Send(packet);

            return base.HandleMessageAsync(incomingPacketStream);
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"New client connected with id: {Id}");

            using var welcomePacket = new LitePacket();
            welcomePacket.WriteString($"Hello {Id}!");
            
            Send(welcomePacket);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Client '{Id}' disconnected.");
        }
    }
}
