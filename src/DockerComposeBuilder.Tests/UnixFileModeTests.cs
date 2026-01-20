using DockerComposeBuilder.Model.Services;
using System;
using Xunit;

namespace DockerComposeBuilder.Tests;

public class UnixFileModeTests
{
    [Theory]
    [InlineData("0400", 256, UnixFileModeNotation.Octal)]
    [InlineData("0440", 288, UnixFileModeNotation.Octal)]
    [InlineData("0444", 292, UnixFileModeNotation.Octal)]
    [InlineData("0644", 420, UnixFileModeNotation.Octal)]
    [InlineData("0755", 493, UnixFileModeNotation.Octal)]
    [InlineData("0777", 511, UnixFileModeNotation.Octal)]
    public void Parse_OctalNotation_ReturnsCorrectValue(string input, int expectedIntValue, UnixFileModeNotation expectedNotation)
    {
        var mode = UnixFileMode.Parse(input);

        Assert.Equal(expectedIntValue, mode.IntValue);
        Assert.Equal(expectedNotation, mode.Notation);
    }

    [Theory]
    [InlineData("0o400", 256, UnixFileModeNotation.OctalWithO)]
    [InlineData("0o440", 288, UnixFileModeNotation.OctalWithO)]
    [InlineData("0o444", 292, UnixFileModeNotation.OctalWithO)]
    [InlineData("0o644", 420, UnixFileModeNotation.OctalWithO)]
    [InlineData("0o755", 493, UnixFileModeNotation.OctalWithO)]
    [InlineData("0o777", 511, UnixFileModeNotation.OctalWithO)]
    [InlineData("0O755", 493, UnixFileModeNotation.OctalWithO)]
    public void Parse_OctalWithONotation_ReturnsCorrectValue(string input, int expectedIntValue, UnixFileModeNotation expectedNotation)
    {
        var mode = UnixFileMode.Parse(input);

        Assert.Equal(expectedIntValue, mode.IntValue);
        Assert.Equal(expectedNotation, mode.Notation);
    }

    [Theory]
    [InlineData("256", 256, UnixFileModeNotation.RawInt)]
    [InlineData("288", 288, UnixFileModeNotation.RawInt)]
    [InlineData("292", 292, UnixFileModeNotation.RawInt)]
    [InlineData("420", 420, UnixFileModeNotation.RawInt)]
    [InlineData("493", 493, UnixFileModeNotation.RawInt)]
    [InlineData("511", 511, UnixFileModeNotation.RawInt)]
    public void Parse_RawIntNotation_ReturnsCorrectValue(string input, int expectedIntValue, UnixFileModeNotation expectedNotation)
    {
        var mode = UnixFileMode.Parse(input);

        Assert.Equal(expectedIntValue, mode.IntValue);
        Assert.Equal(expectedNotation, mode.Notation);
    }

    [Theory]
    [InlineData("04777", 2559)]
    [InlineData("02755", 1517)]
    [InlineData("01755", 1005)]
    [InlineData("06755", 3565)]
    [InlineData("07777", 4095)]
    public void Parse_SetuidSetgidStickyBit_ReturnsCorrectValue(string input, int expectedIntValue)
    {
        var mode = UnixFileMode.Parse(input);

        Assert.Equal(expectedIntValue, mode.IntValue);
        Assert.Equal(UnixFileModeNotation.Octal, mode.Notation);
    }

    [Theory]
    [InlineData("0o4777", 2559)]
    [InlineData("0o2755", 1517)]
    [InlineData("0o1755", 1005)]
    [InlineData("0o7777", 4095)]
    public void Parse_SetuidSetgidStickyBitWithO_ReturnsCorrectValue(string input, int expectedIntValue)
    {
        var mode = UnixFileMode.Parse(input);

        Assert.Equal(expectedIntValue, mode.IntValue);
        Assert.Equal(UnixFileModeNotation.OctalWithO, mode.Notation);
    }

    [Theory]
    [InlineData("2559", 2559)]
    [InlineData("1517", 1517)]
    [InlineData("1005", 1005)]
    [InlineData("4095", 4095)]
    public void Parse_SetuidSetgidStickyBitRawInt_ReturnsCorrectValue(string input, int expectedIntValue)
    {
        var mode = UnixFileMode.Parse(input);

        Assert.Equal(expectedIntValue, mode.IntValue);
        Assert.Equal(UnixFileModeNotation.RawInt, mode.Notation);
    }

    [Theory]
    [InlineData("0800")]
    [InlineData("0888")]
    [InlineData("0999")]
    [InlineData("0o800")]
    [InlineData("0o888")]
    public void Parse_InvalidOctalDigits_ThrowsException(string input)
    {
        Assert.ThrowsAny<Exception>(() => UnixFileMode.Parse(input));
    }

    [Theory]
    [InlineData("800", 800)]
    [InlineData("888", 888)]
    [InlineData("999", 999)]
    [InlineData("789", 789)]
    [InlineData("918", 918)]
    public void Parse_RawIntWithAnyDigits_ParsesAsDecimal(string input, int expectedValue)
    {
        var mode = UnixFileMode.Parse(input);

        Assert.Equal(expectedValue, mode.IntValue);
        Assert.Equal(UnixFileModeNotation.RawInt, mode.Notation);
    }

    [Fact]
    public void Parse_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UnixFileMode.Parse(""));
    }

    [Fact]
    public void Parse_NullString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UnixFileMode.Parse(null!));
    }

    [Theory]
    [InlineData("0400", "0400")]
    [InlineData("0755", "0755")]
    [InlineData("04777", "04777")]
    [InlineData("0o400", "0o400")]
    [InlineData("0o755", "0o755")]
    [InlineData("0o4777", "0o4777")]
    [InlineData("256", "256")]
    [InlineData("493", "493")]
    [InlineData("2559", "2559")]
    public void ToNotationString_PreservesFormat(string input, string expected)
    {
        var mode = UnixFileMode.Parse(input);
        Assert.Equal(expected, mode.ToNotationString());
    }

    [Theory]
    [InlineData("0400", "0400")]
    [InlineData("0o400", "0400")]
    [InlineData("256", "0400")]
    public void ToOctalString_AllNotations_ReturnsOctalFormat(string input, string expected)
    {
        var mode = UnixFileMode.Parse(input);
        Assert.Equal(expected, mode.ToOctalString());
    }

    [Fact]
    public void FromDecimal_ValidValue_CreatesMode()
    {
        var mode = UnixFileMode.FromDecimal(256);

        Assert.Equal(256, mode.IntValue);
        Assert.Equal(UnixFileModeNotation.RawInt, mode.Notation);
    }

    [Fact]
    public void ImplicitConversion_IntToMode_Works()
    {
        UnixFileMode mode = 256;

        Assert.Equal(256, mode.IntValue);
    }

    [Fact]
    public void ImplicitConversion_ModeToInt_Works()
    {
        var mode = UnixFileMode.Parse("0400");

        int value = mode;

        Assert.Equal(256, value);
    }

    [Fact]
    public void ImplicitConversion_Roundtrip_IsEqual()
    {
        UnixFileMode original = 256;

        int intValue = original;
        UnixFileMode roundtripped = intValue;

        Assert.Equal(original, roundtripped);
    }

    [Fact]
    public void Equality_SameValueAndNotation_AreEqual()
    {
        var mode1 = UnixFileMode.Parse("0400");
        var mode2 = UnixFileMode.Parse("0400");

        Assert.Equal(mode1, mode2);
        Assert.True(mode1 == mode2);
    }

    [Fact]
    public void Equality_SameValueDifferentNotation_AreNotEqual()
    {
        var mode1 = UnixFileMode.Parse("0400");
        var mode2 = UnixFileMode.Parse("0o400");
        var mode3 = UnixFileMode.Parse("256");

        Assert.NotEqual(mode1, mode2);
        Assert.NotEqual(mode2, mode3);
        Assert.NotEqual(mode1, mode3);
        Assert.Equal(mode1.IntValue, mode2.IntValue);
        Assert.Equal(mode2.IntValue, mode3.IntValue);
    }

    [Fact]
    public void Equality_DifferentValue_AreNotEqual()
    {
        var mode1 = UnixFileMode.Parse("0400");
        var mode2 = UnixFileMode.Parse("0755");

        Assert.NotEqual(mode1, mode2);
        Assert.True(mode1 != mode2);
    }

    [Fact]
    public void GetHashCode_SameValueAndNotation_SameHash()
    {
        var mode1 = UnixFileMode.Parse("0400");
        var mode2 = UnixFileMode.Parse("0400");

        Assert.Equal(mode1.GetHashCode(), mode2.GetHashCode());
    }

    [Fact]
    public void ParseAsOctetFromInt_ValidValue_Works()
    {
        var mode = UnixFileMode.ParseAsOctetFromInt(755);

        Assert.Equal(493, mode.IntValue);
        Assert.Equal(UnixFileModeNotation.RawInt, mode.Notation);
    }

    [Fact]
    public void ParseAsOctetFromInt_InvalidDigit_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => UnixFileMode.ParseAsOctetFromInt(800));
        Assert.Contains("Invalid octal digit 8", ex.Message);
    }
}
