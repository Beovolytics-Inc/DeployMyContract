using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using DeployMyContract.Core.Contracts;
using DeployMyContract.Core.Helpers;
using Nethereum.Util;

namespace DeployMyContract.Core.Logic
{
    public class ArgumentValueParser : IArgumentValueParser
    {
        public const string ArrayElementDelimiter = ";";

        private static readonly Regex M_HexRegex = new Regex(
            @"^(0x)?[0-9a-f]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex M_IntRegex = new Regex(
            @"^(?<value>-?\d+)(\s*(?<unit>\w+))?$", RegexOptions.Compiled);

        private static readonly Dictionary<string, BigInteger> M_IntMultipliers =
            new Dictionary<string, BigInteger>(StringComparer.InvariantCultureIgnoreCase)
            {
                //Time units
                ["seconds"] = 1,
                ["minutes"] = 60,
                ["hours"] = 60 * 60,
                ["days"] = 60 * 60 * 24,
                ["weeks"] = 60 * 60 * 24 * 7,
                ["years"] = 60 * 60 * 24 * 365,

                //Eth units
                ["ether"] = new BigInteger(1e18),
                ["finney"] = new BigInteger(1e15),
                ["szabo"] = new BigInteger(1e12),
                ["wei"] = 1
            };

        public object Parse(string name, string type, string valueStr)
        {
            var normalizedType = type.ToLowerInvariant().Trim();
            var normalizedValue = (valueStr ?? string.Empty).Trim();
            if (normalizedType.EndsWith("[]"))
                return ParseArray(name, normalizedType, normalizedValue);
            if (normalizedType == "bool")
                return ParseBool(name, normalizedValue);
            if (normalizedType.StartsWith("int"))
                return ParseInt(name, normalizedType, normalizedValue, "int".Length);
            if (normalizedType.StartsWith("uint"))
                return ParseUint(name, normalizedType, normalizedValue);
            if (normalizedType == "address")
                return ParseAddress(name, normalizedValue);
            if (normalizedType == "byte")
                return ParseByte(name, normalizedValue);
            if (normalizedType.StartsWith("bytes"))
                return ParseByteArray(name, normalizedType, normalizedValue);
            return normalizedValue;
        }

        private static byte[] ParseByteArray(string name, string normalizedType, string normalizedValue)
        {
            var bytesCount = normalizedType.Length > 5
                ? int.Parse(normalizedType.Substring(5))
                : (int?) null;
            if (normalizedValue != string.Empty && !M_HexRegex.IsMatch(normalizedValue))
                throw new ArgumentParsingException(name, normalizedValue, "not a byte array value");
            var bytes = HexHelper.FromHex(normalizedValue);
            if (bytesCount != null && bytes.Length != bytesCount)
                throw new ArgumentParsingException(name, normalizedValue,
                    $"{bytesCount}-byte array value must have exactly {bytesCount} bytes");
            return bytes;
        }

        private static byte ParseByte(string name, string normalizedValue)
        {
            if (!byte.TryParse(normalizedValue, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out var byteValue))
                throw new ArgumentParsingException(name, normalizedValue, "not a byte value");
            return byteValue;
        }

        private static string ParseAddress(string name, string normalizedValue)
        {
            var addressUtil = new AddressUtil();
            if (!M_HexRegex.IsMatch(normalizedValue) || !addressUtil.IsValidAddressLength(normalizedValue))
                throw new ArgumentParsingException(name, normalizedValue, "not an Ethereum address");
            if (normalizedValue.ToLowerInvariant() != normalizedValue
                && !addressUtil.IsChecksumAddress(normalizedValue))
                throw new ArgumentParsingException(name, normalizedValue, "invalid address checksum");
            return normalizedValue;
        }

        private static BigInteger ParseUint(string name, string normalizedType, string normalizedValue)
        {
            var intValue = ParseInt(name, normalizedType, normalizedValue, "uint".Length);
            if (intValue < 0)
                throw new ArgumentParsingException(name, normalizedValue, "the value of unsigned integer is negative");
            return intValue;
        }

        private static bool ParseBool(string name, string normalizedValue)
        {
            if (!bool.TryParse(normalizedValue, out var boolValue))
                throw new ArgumentParsingException(name, normalizedValue, "not a boolean value");
            return boolValue;
        }

        private object[] ParseArray(string name, string normalizedType, string normalizedValue)
        {
            if (normalizedValue == string.Empty)
                return new object[0];
            var elementType = normalizedType.Substring(0, normalizedType.Length - 2);
            return normalizedValue.Split(ArrayElementDelimiter.ToCharArray())
                .Select((x, i) => Parse($"{name}[{i}]", elementType, x))
                .ToArray();
        }

        private static BigInteger ParseInt(string name, string type, string value, int bitsStrOffset)
        {
            var bits = type.Length > bitsStrOffset
                ? int.Parse(type.Substring(bitsStrOffset))
                : 256;
            var intMatch = M_IntRegex.Match(value);
            if (!intMatch.Success)
                throw new ArgumentParsingException(name, value, "not an integer value");
            var intResult = BigInteger.Parse(intMatch.Groups["value"].Value);
            if (intMatch.Groups["unit"].Success)
            {
                if (!M_IntMultipliers.TryGetValue(intMatch.Groups["unit"].Value, out var multiplier))
                    throw new ArgumentParsingException(name, value, "unknown multiplier suffix");
                intResult *= multiplier;
            }
            if (intResult.ToByteArray().Reverse().SkipWhile(x => x == 0).Count() > bits / 8)
                throw new ArgumentParsingException(name, value, $"value is too big to fit in {bits} bits");
            return intResult;
        }
    }
}
