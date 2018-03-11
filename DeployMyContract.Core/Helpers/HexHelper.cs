using System.Globalization;
using System.Linq;
using System.Text;

namespace DeployMyContract.Core.Helpers
{
    public static class HexHelper
    {
        private const string Prefix = "0x";

        public static string ToHex(byte[] bytes, bool addPrefix = false)
        {
            if (bytes == null || !bytes.Any())
                return string.Empty;
            var builder = new StringBuilder();
            if (addPrefix)
                builder.Append(Prefix);
            foreach (var @byte in bytes)
                builder.AppendFormat("{0:x2}", @byte);
            return builder.ToString();
        }

        public static byte[] FromHex(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return new byte[0];

            if (str.StartsWith(Prefix))
            {
                if (str.Length == Prefix.Length)
                    return new byte[0];
                str = str.Substring(Prefix.Length);
            }
            if (str.Length % 2 != 0)
                str = "0" + str;
            var bytes = new byte[str.Length / 2];
            for (var i = 0; i < str.Length; i += 2)
                bytes[i / 2] = byte.Parse(str.Substring(i, 2), NumberStyles.HexNumber);
            return bytes;
        }
    }
}
