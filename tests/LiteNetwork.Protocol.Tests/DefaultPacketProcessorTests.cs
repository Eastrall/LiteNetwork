﻿using Bogus;
using LiteNetwork.Protocol.Abstractions;
using System;
using Xunit;

namespace LiteNetwork.Protocol.Tests;

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
    public void DefaultPacketProcessorNeverIncludeHeaderTest()
    {
        Assert.False(_packetProcessor.IncludeHeader);
    }
}
