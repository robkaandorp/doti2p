using Xunit;

namespace DotI2p.Tests;

public class CommandResponseTests
{
    [Fact]
    public void Constructor_StandardResponse_ParsesResponseType()
    {
        var response = new CommandResponse("HELLO REPLY RESULT=OK VERSION=3.1");

        Assert.Equal("HELLO REPLY", response.Response);
    }

    [Fact]
    public void Constructor_StandardResponse_ParsesKeyValuePairs()
    {
        var response = new CommandResponse("HELLO REPLY RESULT=OK VERSION=3.1");

        Assert.Equal("OK", response.ResponseDictionary["RESULT"]);
        Assert.Equal("3.1", response.ResponseDictionary["VERSION"]);
    }

    [Fact]
    public void Constructor_StandardResponse_PreservesOriginalResponse()
    {
        const string original = "HELLO REPLY RESULT=OK VERSION=3.1";

        var response = new CommandResponse(original);

        Assert.Equal(original, response.OriginalResponse);
    }

    [Fact]
    public void Constructor_QuotedValue_ParsesWithoutQuotes()
    {
        var response = new CommandResponse("HELLO REPLY RESULT=ERROR MESSAGE=\"There was an error\"");

        Assert.Equal("HELLO REPLY", response.Response);
        Assert.Equal("ERROR", response.ResponseDictionary["RESULT"]);
        Assert.Equal("There was an error", response.ResponseDictionary["MESSAGE"]);
    }

    [Fact]
    public void Constructor_EmptyQuotedValue_ParsesAsEmptyString()
    {
        var response = new CommandResponse("SOME RESPONSE KEY=\"\"");

        Assert.Equal("", response.ResponseDictionary["KEY"]);
    }

    [Fact]
    public void Constructor_ValueContainingEquals_PreservesFullValue()
    {
        var response = new CommandResponse("SOME RESPONSE KEY=val=ue");

        Assert.Equal("val=ue", response.ResponseDictionary["KEY"]);
    }

    [Fact]
    public void Constructor_NoKeyValuePairs_ReturnsEmptyDictionary()
    {
        var response = new CommandResponse("HELLO REPLY");

        Assert.Equal("HELLO REPLY", response.Response);
        Assert.Empty(response.ResponseDictionary);
    }

    [Fact]
    public void Constructor_SingleWord_ReturnsEmptyDictionary()
    {
        var response = new CommandResponse("HELLO");

        Assert.Equal("HELLO", response.Response);
        Assert.Empty(response.ResponseDictionary);
    }

    [Theory]
    [InlineData("SESSION STATUS RESULT=OK DESTINATION=abc123", "SESSION STATUS", 2)]
    [InlineData("DEST REPLY PUB=pubkey PRIV=privkey", "DEST REPLY", 2)]
    [InlineData("NAMING REPLY RESULT=OK NAME=host.i2p VALUE=dest", "NAMING REPLY", 3)]
    public void Constructor_VariousResponses_ParsesCorrectly(
        string input, string expectedResponse, int expectedPairCount)
    {
        var response = new CommandResponse(input);

        Assert.Equal(expectedResponse, response.Response);
        Assert.Equal(expectedPairCount, response.ResponseDictionary.Count);
    }

    [Fact]
    public void Constructor_QuotedValueWithMultipleSpaces_PreservesAllSpaces()
    {
        var response = new CommandResponse("STREAM STATUS RESULT=I2P_ERROR MESSAGE=\"Could not reach the peer\"");

        Assert.Equal("Could not reach the peer", response.ResponseDictionary["MESSAGE"]);
    }

    [Fact]
    public void Constructor_MultipleConsecutiveSpacesBetweenPairs_SkipsEmptyTokens()
    {
        var response = new CommandResponse("HELLO REPLY  RESULT=OK");

        Assert.Equal("OK", response.ResponseDictionary["RESULT"]);
    }
}
