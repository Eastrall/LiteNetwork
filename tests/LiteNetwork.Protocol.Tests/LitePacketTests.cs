using Bogus;
using System;
using System.Linq;
using Xunit;

namespace LiteNetwork.Protocol.Tests
{
    public sealed class LitePacketTests
    {
        private readonly Randomizer _randomizer;

        public LitePacketTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void CreateNetPacketTest()
        {
            const int HeaderSize = sizeof(int);
            const int DataSize = sizeof(short) + sizeof(float);
            var shortValue = _randomizer.Short();
            var floatValue = _randomizer.Float();

            var packet = new LitePacket();

            packet.Write(shortValue);
            packet.Write(floatValue);

            Assert.Equal(LitePacketMode.Write, packet.Mode);
            Assert.NotNull(packet.Buffer);
            Assert.Equal(HeaderSize + DataSize, packet.Buffer.Length);
            Assert.Equal(DataSize, packet.ContentLength);
            Assert.Equal(DataSize, BitConverter.ToInt32(packet.Buffer, 0));
            Assert.Equal(shortValue, BitConverter.ToInt16(packet.Buffer, HeaderSize));
            Assert.Equal(floatValue, BitConverter.ToSingle(packet.Buffer, HeaderSize + sizeof(short)));

            packet.Dispose();
        }

        [Fact]
        public void CreateEmptyNetPacketTest()
        {
            var packet = new LitePacket();

            Assert.Equal(LitePacketMode.Write, packet.Mode);
            Assert.NotNull(packet.Buffer);
            Assert.Equal(sizeof(int), packet.Buffer.Length);
            Assert.Equal(0, BitConverter.ToInt32(packet.Buffer, 0));

            packet.Dispose();
        }

        [Fact]
        public void CreateNetPacketFromBufferTest()
        {
            var shortValue = _randomizer.Short();
            var floatValue = _randomizer.Float();
            const int contentSize = sizeof(short) + sizeof(float);
            var data = BitConverter.GetBytes(contentSize)
                            .Concat(BitConverter.GetBytes(shortValue))
                            .Concat(BitConverter.GetBytes((float)floatValue))
                            .ToArray();

            var packet = new LitePacket(data);
            var packetContentSize = packet.Read<int>();
            var shortValuePacket = packet.Read<short>();
            var floatValuePacket = packet.Read<float>();

            Assert.Equal(LitePacketMode.Read, packet.Mode);
            Assert.NotNull(packet.Buffer);
            Assert.Equal(data, packet.Buffer);
            Assert.Equal(contentSize, BitConverter.ToInt32(packet.Buffer, 0));
            Assert.Equal(contentSize, packetContentSize);

            Assert.Equal(shortValue, BitConverter.ToInt16(packet.Buffer, sizeof(int)));
            Assert.Equal(shortValue, shortValuePacket);

            Assert.Equal(floatValue, BitConverter.ToSingle(packet.Buffer, sizeof(int) + sizeof(short)));
            Assert.Equal(floatValue, floatValuePacket);

            packet.Dispose();
        }

        [Fact]
        public void CreatePacketWithCustomHeaderSize()
        {
            const int HeaderSize = sizeof(ushort);
            const int DataSize = sizeof(short) + sizeof(float);
            var shortValue = _randomizer.Short();
            var floatValue = _randomizer.Float();

            var packet = new LitePacket(HeaderSize);

            packet.Write(shortValue);
            packet.Write(floatValue);

            Assert.Equal(LitePacketMode.Write, packet.Mode);
            Assert.NotNull(packet.Buffer);
            Assert.Equal(HeaderSize + DataSize, packet.Buffer.Length);
            Assert.Equal(DataSize, packet.ContentLength);
            Assert.Equal(DataSize, BitConverter.ToInt16(packet.Buffer, 0));
            Assert.Equal(shortValue, BitConverter.ToInt16(packet.Buffer, HeaderSize));
            Assert.Equal(floatValue, BitConverter.ToSingle(packet.Buffer, HeaderSize + sizeof(short)));

            packet.Dispose();
        }
    }
}
