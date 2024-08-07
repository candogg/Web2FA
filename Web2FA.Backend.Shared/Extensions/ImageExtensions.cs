﻿using System.Drawing;
using System.Drawing.Imaging;

namespace Web2FA.Backend.Shared.Extensions
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public static class ImageExtensions
    {
        public static byte[] ToByteArray(this Bitmap image, ImageFormat format)
        {
            using MemoryStream ms = new();

            try
            {
                image.Save(ms, format);
            }
            catch
            { }

            return ms.ToArray();
        }
    }
}
