using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    public sealed class CaptchaService : ServiceSingularBase<CaptchaService>
    {
        public string? MakeCaptchaImage(string captchaValue, int width, int height, string fontFamilyName)
        {
            try
            {
                var cImage = new System.Drawing.Bitmap(width, height);
                var gr = System.Drawing.Graphics.FromImage(cImage);

                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                var recF = new System.Drawing.RectangleF(0, 0, width, height);

                var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallConfetti, System.Drawing.Color.LightGray, System.Drawing.Color.White);
                gr.FillRectangle(brush, recF);

                System.Drawing.SizeF textSize;
                System.Drawing.Font font;
                var fontSize = height + 1;

                do
                {
                    fontSize -= 1;
                    font = new System.Drawing.Font(fontFamilyName, fontSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                    textSize = gr.MeasureString(captchaValue, font);
                }
                while (textSize.Width > width || textSize.Height > height);

                var sFormat = new System.Drawing.StringFormat
                {
                    Alignment = System.Drawing.StringAlignment.Center,
                    LineAlignment = System.Drawing.StringAlignment.Center
                };

                var gPath = new System.Drawing.Drawing2D.GraphicsPath();
                gPath.AddString(captchaValue, font.FontFamily, 1, font.Size, recF, sFormat);

                var rnd = new Random();

                System.Drawing.PointF[] pts = { new((float)rnd.Next(width) / 4, (float)rnd.Next(height) / 4), new(width - (float)rnd.Next(width) / 4, (float)rnd.Next(height) / 4), new((float)rnd.Next(width) / 4, height - (float)rnd.Next(height) / 4), new(width - (float)rnd.Next(width) / 4, height - (float)rnd.Next(height) / 4) };

                var mat = new System.Drawing.Drawing2D.Matrix();
                gPath.Warp(pts, recF, mat, System.Drawing.Drawing2D.WarpMode.Perspective, 0);

                brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LargeConfetti, System.Drawing.Color.LightGray, System.Drawing.Color.DarkGray);
                gr.FillPath(brush, gPath);

                var maxDimension = Math.Max(width, height);

                for (var i = 0; i <= width * height / 20; i++)
                {
                    var X = rnd.Next(width);
                    var Y = rnd.Next(height);
                    var W = rnd.Next(maxDimension) / 50;
                    var H = rnd.Next(maxDimension) / 50;

                    gr.FillEllipse(brush, X, Y, W, H);
                }

                for (var i = 1; i <= 5; i++)
                {
                    var x1 = rnd.Next(width);
                    var y1 = rnd.Next(height);
                    var x2 = rnd.Next(width);
                    var y2 = rnd.Next(height);

                    gr.DrawLine(System.Drawing.Pens.DarkGray, x1, y1, x2, y2);
                }

                for (var i = 1; i <= 5; i++)
                {
                    var x1 = rnd.Next(width);
                    var y1 = rnd.Next(height);
                    var x2 = rnd.Next(width);
                    var y2 = rnd.Next(height);

                    gr.DrawLine(System.Drawing.Pens.LightGray, x1, y1, x2, y2);
                }

                gPath.Dispose();
                brush.Dispose();
                font.Dispose();
                gr.Dispose();

                return Convert.ToBase64String(cImage.ToByteArray(System.Drawing.Imaging.ImageFormat.Bmp));
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return null;
        }
    }
}
