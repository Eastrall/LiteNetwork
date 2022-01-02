using Bogus;
using LiteNetwork.Protocol.Abstractions;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace LiteNetwork.Protocol.Tests
{
    public sealed class DefaultPacketProcessorTests
    {
        private readonly Faker _faker;
        private readonly ILitePacketProcessor _packetProcessor;

        public DefaultPacketProcessorTests()
        {
            _faker = new Faker();
            _packetProcessor = new LitePacketProcessor();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(23)]
        [InlineData(0x4A)]
        [InlineData(0)]
        [InlineData(-1)]
        public void ParsePacketHeaderTest(int headerValue)
        {
            var headerBuffer = BitConverter.GetBytes(headerValue);
            int packetSize = _packetProcessor.GetMessageLength(headerBuffer);

            Assert.Equal(headerValue, packetSize);
        }

        [Fact]
        public void CreatePacketStreamFromDefaultProcessorTest()
        {
            string randomString = _faker.Lorem.Sentence(3);
            var messageData = BitConverter.GetBytes(randomString.Length).Concat(Encoding.UTF8.GetBytes(randomString)).ToArray();

            ILitePacketStream packetStream = _packetProcessor.CreatePacket(messageData);

            Assert.NotNull(packetStream);
            string packetStreamString = packetStream.Read<string>();
            Assert.NotNull(packetStreamString);
            Assert.Equal(randomString, packetStreamString);
        }

        [Fact]
        public void DefaultPacketProcessorNeverIncludeHeaderTest()
        {
            Assert.False(_packetProcessor.IncludeHeader);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(23)]
        [InlineData(0x4A)]
        [InlineData(0)]
        public void DefaultPacketProcessorCanHaveConfigurableHeaderSize(short headerValue)
        {
            var packetProcessor = new LitePacketProcessor(sizeof(short));

            var headerBuffer = BitConverter.GetBytes(headerValue);
            int packetSize = packetProcessor.GetMessageLength(headerBuffer);

            Assert.Equal(headerValue, packetSize);
        }
    }
}
