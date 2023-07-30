using System.Buffers.Binary;
using Xunit;

namespace LiteNetwork.Protocol.Tests;

public sealed class DefaultPacketProcessorTests
{
    private readonly LitePacketProcessor _littleEndianPacketProcessor;
    private readonly LitePacketProcessor _bigEndianPacketProcessor;

    public DefaultPacketProcessorTests()
    {
        _littleEndianPacketProcessor = new LitePacketProcessor(true);
        _bigEndianPacketProcessor = new LitePacketProcessor(false);
    }

    [Theory]
    [InlineData(35)]
    [InlineData(23)]
    [InlineData(0x4A)]
    [InlineData(0)]
    [InlineData(-1)]
    public void ParsePacketHeaderLittleEndianTest(int headerValue)
    {
        Assert.True(_littleEndianPacketProcessor.IsLittleEndianMode);

        var headerBuffer = new byte[sizeof(int)];
        BinaryPrimitives.WriteInt32LittleEndian(headerBuffer, headerValue);

        int packetSize = _littleEndianPacketProcessor.GetMessageLength(headerBuffer);

        Assert.Equal(headerValue, packetSize);
    }

    [Theory]
    [InlineData(35)]
    [InlineData(23)]
    [InlineData(0x4A)]
    [InlineData(0)]
    [InlineData(-1)]
    public void ParsePacketHeaderBigEndianTest(int headerValue)
    {
        Assert.False(_bigEndianPacketProcessor.IsLittleEndianMode);

        var headerBuffer = new byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(headerBuffer, headerValue);

        int packetSize = _bigEndianPacketProcessor.GetMessageLength(headerBuffer);

        Assert.Equal(headerValue, packetSize);
    }

    [Fact]
    public void DefaultPacketProcessorNeverIncludeHeaderTest()
    {
        Assert.False(_littleEndianPacketProcessor.IncludeHeader);
    }
}
