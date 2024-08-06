using System.Text;
using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Backend.Shared.Extensions
{
    public static class StringExtensions
    {
        private static readonly Dictionary<char, char> turkishToEnglishMap = new()
        {
                { 'ç', 'C' }, { 'Ç', 'C' },
                { 'ğ', 'G' }, { 'Ğ', 'G' },
                { 'ı', 'I' }, { 'İ', 'I' },
                { 'ö', 'O' }, { 'Ö', 'O' },
                { 'ş', 'S' }, { 'Ş', 'S' },
                { 'ü', 'U' }, { 'Ü', 'U' }
            };

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static string AsEncrypted(this string str)
        {
            if (str.IsNullOrEmpty()) return str;

            return EncryptionService.DerivedObject.EncryptString(str);
        }

        public static string AsDecrypted(this string str)
        {
            if (str.IsNullOrEmpty()) return str;

            return EncryptionService.DerivedObject.DecryptString(str);
        }

        public static string AsHash(this string str)
        {
            if (str.IsNullOrEmpty()) return str;

            return EncryptionService.DerivedObject.GetSignature(str);
        }

        public static string ToUpper(this string str, bool useEnglish = false)
        {
            if (str.Trim().IsNullOrEmpty())
            {
                return str;
            }

            var sb = new StringBuilder();

            foreach (char c in str)
            {
                if (useEnglish)
                {
                    if (turkishToEnglishMap.TryGetValue(c, out char convertedChar))
                    {
                        sb.Append(convertedChar);
                    }
                    else
                    {
                        sb.Append(char.ToUpperInvariant(c));
                    }
                }
                else
                {
                    sb.Append(c.ToString().ToUpper());
                }
            }

            return sb.ToString();
        }

        public static string ToUpperEnglish(this string str)
        {
            if (str.IsNullOrEmpty()) return string.Empty;

            return str.ToUpper(true);
        }

        public static string ToLowerEnglish(this string str)
        {
            if (str.IsNullOrEmpty()) return string.Empty;

            return str.ToLower(true);
        }

        public static string ToLower(this string str, bool useEnglish)
        {
            if (str.IsNullOrEmpty()) return string.Empty;

            if (str.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (char c in str)
            {
                if (useEnglish)
                {
                    if (turkishToEnglishMap.TryGetValue(c, out char convertedChar))
                    {
                        sb.Append(convertedChar);
                    }
                    else
                    {
                        sb.Append(char.ToLowerInvariant(c));
                    }
                }
                else
                {
                    sb.Append(c.ToString().ToLowerInvariant());
                }
            }

            return sb.ToString();
        }
    }
}
