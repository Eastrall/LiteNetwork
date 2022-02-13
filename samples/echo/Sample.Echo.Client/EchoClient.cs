using LiteNetwork.Client;
using LiteNetwork.Protocol.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sample.Echo.Client
{
    public class EchoClient : LiteClient
    {
        public EchoClient(LiteClientOptions options, IServiceProvider serviceProvider = null) 
            : base(options, serviceProvider)
        {
        }

        public override Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            string message = incomingPacketStream.Read<string>();

            Console.WriteLine($"Received from server: {message}");

            return Task.CompletedTask;
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Client connected.");
            base.OnConnected();
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected");
            base.OnDisconnected();
        }
    }
}
