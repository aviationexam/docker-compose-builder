using System;
using System.Diagnostics;

namespace DockerComposeBuilder.Model.Services;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly record struct UnixFileMode
{
    private readonly int _value;

    private UnixFileMode(int value, UnixFileModeNotation notation)
    {
        _value = value;
        Notation = notation;
    }

    public int IntValue => _value;

    public UnixFileModeNotation Notation { get; init; }

    private string DebuggerDisplay => ToNotationString(UnixFileModeNotation.Octal);

    public static UnixFileMode FromDecimal(
        int decimalValue,
        UnixFileModeNotation notation = UnixFileModeNotation.RawInt
    )
    {
        ValidateOctalDigits(decimalValue);
        return new UnixFileMode(decimalValue, notation);
    }

    public static UnixFileMode ParseAsOctetFromInt(
        int octalAsDecimal,
        UnixFileModeNotation notation = UnixFileModeNotation.RawInt
    )
    {
        var result = 0;
        var multiplier = 1;
        var temp = octalAsDecimal;

        while (temp > 0)
        {
            var digit = temp % 10;
            if (digit > 7)
            {
                throw new ArgumentException(
                    $"Invalid octal digit {digit} in value {octalAsDecimal}. Each digit must be 0-7.",
                    nameof(octalAsDecimal));
            }

            result += digit * multiplier;
            multiplier *= 8;
            temp /= 10;
        }

        return new UnixFileMode(result, notation);
    }

    public static UnixFileMode Parse(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(value));
        }

        if (value.StartsWith("0o", StringComparison.OrdinalIgnoreCase) && value.Length > 2)
        {
            return new UnixFileMode(Convert.ToInt32(value[2..], 8), UnixFileModeNotation.OctalWithO);
        }

        if (value.StartsWith("0") && value.Length > 1)
        {
            return new UnixFileMode(Convert.ToInt32(value, 8), UnixFileModeNotation.Octal);
        }

        return new UnixFileMode(int.Parse(value), UnixFileModeNotation.RawInt);
    }

    public string ToNotationString(UnixFileModeNotation notation) => notation switch
    {
        UnixFileModeNotation.OctalWithO => $"0o{Convert.ToString(_value, 8)}",
        UnixFileModeNotation.RawInt => IntValue.ToString(),
        UnixFileModeNotation.Octal => ToOctalString(),
        _ => throw new InvalidOperationException($"Unknown notation {notation}")
    };

    public string ToNotationString() => ToNotationString(Notation);

    public string ToOctalString() => $"0{Convert.ToString(_value, 8)}";

    public static implicit operator int(UnixFileMode mode) => mode._value;

    public static implicit operator UnixFileMode(int decimalValue) => FromDecimal(decimalValue, UnixFileModeNotation.Octal);

    private static void ValidateOctalDigits(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "File mode cannot be negative.");
        }

        if (value > 0777)
        {
            var temp = value;
            while (temp > 0)
            {
                var digit = temp % 10;
                if (digit > 7)
                {
                    throw new ArgumentException(
                        $"Invalid octal digit {digit} in value {value}. Each digit must be 0-7.",
                        nameof(value));
                }

                temp /= 10;
            }
        }
    }
}
