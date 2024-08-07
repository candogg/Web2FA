using System.Security.Cryptography;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class RandomService : ServiceSingularBase<RandomService>
    {
        public int GetValue(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue) return -1;

            var diff = (long)maxExclusiveValue - minValue;
            var upperBound = uint.MaxValue / diff * diff;

            uint ui;

            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);

            return (int)(minValue + ui % diff);
        }

        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            var buffer = new byte[bytesNumber];
            RandomNumberGenerator.Create().GetBytes(buffer);
            return buffer;
        }
    }
}
