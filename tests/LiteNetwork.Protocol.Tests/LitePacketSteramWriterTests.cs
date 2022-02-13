using Bogus;
using LiteNetwork.Protocol.Abstractions;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace LiteNetwork.Protocol.Tests
{
    public sealed class LitePacketSteramWriterTests
    {
        private readonly Randomizer _randomizer;

        public LitePacketSteramWriterTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void PacketStreamWriteNonPrimitiveTest()
        {
            using ILitePacketStream packetStream = new LitePacketStream();

            Assert.Throws<NotImplementedException>(() => packetStream.Write<object>(new object()));
        }

        [Fact]
        public void PacketStreamWriteWhenReadOnlyTest()
        {
            using (ILitePacketStream packetStream = new LitePacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Write<byte>(_randomizer.Byte()));
            }
        }

        [Fact]
        public void PacketStreamWriteByteTest()
        {
            var byteValue = _randomizer.Byte();

            PacketStreamWritePrimitive(byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamWriteByteMethodTest()
        {
            var byteValue = _randomizer.Byte();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteByte(value), byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamWriteSByteTest()
        {
            var sbyteValue = _randomizer.SByte();

            PacketStreamWritePrimitive(sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamWriteSByteMethodTest()
        {
            var sbyteValue = _randomizer.SByte();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteSByte(value), sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamWriteBooleanTest()
        {
            var booleanValue = _randomizer.Bool();

            PacketStreamWritePrimitive(booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamWriteBooleanMethodTest()
        {
            var booleanValue = _randomizer.Bool();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteBoolean(value), booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamWriteCharTest()
        {
            var charValue = _randomizer.Char(max: 'z');

            PacketStreamWritePrimitive(charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamWriteCharMethodTest()
        {
            var charValue = _randomizer.Char(max: 'z');

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteChar(value), charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamWriteShortTest()
        {
            var shortValue = _randomizer.Short();

            PacketStreamWritePrimitive(shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamWriteShortMethodTest()
        {
            var shortValue = _randomizer.Short();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteInt16(value), shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamWriteUShortTest()
        {
            var ushortValue = _randomizer.UShort();

            PacketStreamWritePrimitive(ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamWriteUShortMethodTest()
        {
            var ushortValue = _randomizer.UShort();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteUInt16(value), ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamWriteIntTest()
        {
            var intValue = _randomizer.Int();

            PacketStreamWritePrimitive(intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamWriteIntMethodTest()
        {
            var intValue = _randomizer.Int();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteInt32(value), intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamWriteUIntTest()
        {
            var uintValue = _randomizer.UInt();

            PacketStreamWritePrimitive(uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamWriteUIntMethodTest()
        {
            var uintValue = _randomizer.UInt();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteUInt32(value), uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamWriteLongTest()
        {
            var longValue = _randomizer.Long();

            PacketStreamWritePrimitive(longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamWriteLongMethodTest()
        {
            var longValue = _randomizer.Long();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteInt64(value), longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamWriteULongTest()
        {
            var ulongValue = _randomizer.ULong();

            PacketStreamWritePrimitive(ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamWriteULongMethodTest()
        {
            var ulongValue = _randomizer.ULong();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteUInt64(value), ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamWriteFloatTest()
        {
            var floatValue = _randomizer.Float();

            PacketStreamWritePrimitive(floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamWriteFloatMethodTest()
        {
            var floatValue = _randomizer.Float();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteSingle(value), floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamWriteDoubleTest()
        {
            var doubleValue = _randomizer.Double();

            PacketStreamWritePrimitive(doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamWriteDoubleMethodTest()
        {
            var doubleValue = _randomizer.Double();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteDouble(value), doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamWriteStringTest()
        {
            var stringValue = new Faker().Lorem.Sentence();
            var stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamWritePrimitive(stringValue, stringValueArray, adjustBuffer: false);
        }

        [Fact]
        public void PacketStreamWriteStringMethodTest()
        {
            var stringValue = new Faker().Lorem.Sentence();
            var stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteString(value), stringValue, stringValueArray, adjustBuffer: false);
        }

        [Fact]
        public void PacketStreamWriteByteArrayTest()
        {
            var buffer = _randomizer.Bytes(_randomizer.Byte());

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteBytes(value), buffer, buffer, adjustBuffer: false);
        }

        private void PacketStreamWritePrimitive<T>(T valueToWrite, byte[] expectedByteArray, bool adjustBuffer = true)
        {
            using (var packetStream = new LitePacketStream())
            {
                Assert.Equal(LitePacketMode.Write, packetStream.Mode);

                packetStream.Write(valueToWrite);

                var adjustedBuffer = adjustBuffer ? expectedByteArray.Take(Marshal.SizeOf<T>()).ToArray() : expectedByteArray;

                Assert.Equal(adjustedBuffer, packetStream.Buffer);
            }
        }

        private void PacketStreamWritePrimitiveMethod<T>(Action<LitePacketStream, T> method, T valueToWrite, byte[] expectedByteArray, bool adjustBuffer = true)
        {
            using (var packetStream = new LitePacketStream())
            {
                Assert.Equal(LitePacketMode.Write, packetStream.Mode);

                method(packetStream, valueToWrite);

                var adjustedBuffer = adjustBuffer ? expectedByteArray.Take(Marshal.SizeOf<T>()).ToArray() : expectedByteArray;

                Assert.Equal(adjustedBuffer, packetStream.Buffer);
            }
        }
    }
}
