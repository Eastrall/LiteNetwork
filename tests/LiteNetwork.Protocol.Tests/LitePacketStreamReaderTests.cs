using Bogus;
using LiteNetwork.Protocol.Abstractions;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace LiteNetwork.Protocol.Tests
{
    public sealed class LitePacketStreamReaderTests
    {
        private readonly Randomizer _randomizer;

        public LitePacketStreamReaderTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void PacketStreamReadNonPrimitiveTest()
        {
            using (var packetStream = new LitePacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<NotImplementedException>(() => packetStream.Read<object>());
            }
        }

        [Fact]
        public void PacketStreamReadWhenWriteOnlyTest()
        {
            using (var packetStream = new LitePacketStream())
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Read<byte>());
            }
        }

        [Fact]
        public void PacketStreamReadByteTest()
        {
            var byteValue = _randomizer.Byte();

            PacketStreamReadTest(byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamReadByteMethodTest()
        {
            var byteValue = _randomizer.Byte();

            PacketStreamReadMethod<byte>(stream => stream.ReadByte(), byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamReadSByteTest()
        {
            var sbyteValue = _randomizer.SByte();

            PacketStreamReadTest(sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamReadSByteMethodTest()
        {
            var sbyteValue = _randomizer.SByte();

            PacketStreamReadMethod<sbyte>(stream => stream.ReadSByte(), sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamReadBooleanTest()
        {
            var booleanValue = _randomizer.Bool();

            PacketStreamReadTest(booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamReadBooleanMethodTest()
        {
            var booleanValue = _randomizer.Bool();

            PacketStreamReadMethod<bool>(stream => stream.ReadBoolean(), booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamReadCharTest()
        {
            var charValue = _randomizer.Char(max: 'z');

            PacketStreamReadTest(charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamReadCharMethodTest()
        {
            var charValue = _randomizer.Char(max: 'z');

            PacketStreamReadMethod<char>(stream => stream.ReadChar(), charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamReadShortTest()
        {
            var shortValue = _randomizer.Short();

            PacketStreamReadTest(shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamReadShortMethodTest()
        {
            var shortValue = _randomizer.Short();

            PacketStreamReadMethod<short>(stream => stream.ReadInt16(), shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamReadUShortTest()
        {
            var ushortValue = _randomizer.UShort();

            PacketStreamReadTest(ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamReadUShortMethodTest()
        {
            var ushortValue = _randomizer.UShort();

            PacketStreamReadMethod<ushort>(stream => stream.ReadUInt16(), ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamReadIntTest()
        {
            var intValue = _randomizer.Int();

            PacketStreamReadTest(intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamReadIntMethodTest()
        {
            var intValue = _randomizer.Int();

            PacketStreamReadMethod<int>(stream => stream.ReadInt32(), intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamReadUIntTest()
        {
            var uintValue = _randomizer.UInt();

            PacketStreamReadTest(uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamReadUIntMethodTest()
        {
            var uintValue = _randomizer.UInt();

            PacketStreamReadMethod<uint>(stream => stream.ReadUInt32(), uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamReadLongTest()
        {
            var longValue = _randomizer.Long();

            PacketStreamReadTest(longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamReadLongMethodTest()
        {
            var longValue = _randomizer.Long();

            PacketStreamReadMethod<long>(stream => stream.ReadInt64(), longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamReadULongTest()
        {
            var ulongValue = _randomizer.ULong();

            PacketStreamReadTest(ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamReadULongMethodTest()
        {
            var ulongValue = _randomizer.ULong();

            PacketStreamReadMethod<ulong>(stream => stream.ReadUInt64(), ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamReadFloatTest()
        {
            var floatValue = _randomizer.Float();

            PacketStreamReadTest(floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamReadFloatMethodTest()
        {
            var floatValue = _randomizer.Float();

            PacketStreamReadMethod<float>(stream => stream.ReadSingle(), floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamReadDoubleTest()
        {
            var doubleValue = _randomizer.Double();

            PacketStreamReadTest(doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamReadDoubleMethodTest()
        {
            var doubleValue = _randomizer.Double();

            PacketStreamReadMethod<double>(stream => stream.ReadDouble(), doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamReadStringTest()
        {
            var stringValue = new Faker().Lorem.Sentence();
            var stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamReadTest(stringValue, stringValueArray, adjustBuffer: false);
        }

        [Fact]
        public void PacketStreamReadStringMethodTest()
        {
            var stringValue = new Faker().Lorem.Sentence();
            var stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamReadMethod<string>(stream => stream.ReadString(), stringValue, stringValueArray, adjustBuffer: false);
        }

        private void PacketStreamReadTest<T>(T expectedValue, byte[] valueAsBytes, bool adjustBuffer = true)
        {
            var adjustedBuffer = adjustBuffer ? valueAsBytes.Take(Marshal.SizeOf<T>()).ToArray() : valueAsBytes;

            using (var packetStream = new LitePacketStream(adjustedBuffer))
            {
                Assert.Equal(LitePacketMode.Read, packetStream.Mode);
                Assert.False(packetStream.IsEndOfStream);

                var readValue = packetStream.Read<T>();

                Assert.Equal(expectedValue, readValue);
                Assert.True(packetStream.IsEndOfStream);
            }
        }

        private void PacketStreamReadMethod<T>(Func<LitePacketStream, T> method, T expectedValue, byte[] valueAsBytes, bool adjustBuffer = true)
        {
            var adjustedBuffer = adjustBuffer ? valueAsBytes.Take(Marshal.SizeOf<T>()).ToArray() : valueAsBytes;

            using (var packetStream = new LitePacketStream(adjustedBuffer))
            {
                Assert.Equal(LitePacketMode.Read, packetStream.Mode);
                Assert.False(packetStream.IsEndOfStream);

                T readValue = method(packetStream);

                Assert.Equal(expectedValue, readValue);
                Assert.True(packetStream.IsEndOfStream);
            }
        }

        [Fact]
        public void PacketStreamReadNonPrimitiveArrayTest()
        {
            using (var packetStream = new LitePacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<NotImplementedException>(() => packetStream.Read<object>(_randomizer.Byte(min: 1)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void PacketStreamReadArrayWithInvalidAmountTest(int amount)
        {
            using (var packetStream = new LitePacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<ArgumentException>(() => packetStream.Read<byte>(amount));
            }
        }

        [Fact]
        public void PacketStreamReadArrayWhenWriteOnlyTest()
        {
            using (var packetStream = new LitePacketStream())
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Read<byte>(_randomizer.Byte(min: 1)));
            }
        }

        [Fact]
        public void PacketStreamReadByteArrayTest()
        {
            var buffer = _randomizer.Bytes(_randomizer.Byte());
            var amount = _randomizer.Int(min: 1, max: buffer.Length);
            var expectedBuffer = buffer.Take(amount).ToArray();

            using (var packetStream = new LitePacketStream(buffer))
            {
                byte[] readBuffer = packetStream.Read<byte>(amount);

                Assert.Equal(amount, readBuffer.Length);
                Assert.Equal(expectedBuffer, readBuffer);
            }
        }
    }
}
