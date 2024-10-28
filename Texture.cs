using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ReadHelper
{
    public class Texture
    {

        public static Texture2D PixelTexture;
        public static Texture2D CornerTexture;
        public static GraphicsDevice GraphicsDevice;
        public Texture(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            PixelTexture = new Texture2D(graphicsDevice, 1, 1);
            PixelTexture.SetData(new[] { Microsoft.Xna.Framework.Color.White });


            CornerTexture = new Texture2D(graphicsDevice, 100, 100);
            Microsoft.Xna.Framework.Color[] colors = new Microsoft.Xna.Framework.Color[CornerTexture.Width * CornerTexture.Height];
            int fillCol = CornerTexture.Width - 1;
            for (int i = 0; i < CornerTexture.Width; i++)
            {
                for (int ii = 0; ii < CornerTexture.Height; ii++)
                {
                    if (ii < fillCol) continue;
                    colors[i * CornerTexture.Width + ii] = Microsoft.Xna.Framework.Color.White;
                }
                fillCol--;
            }
            CornerTexture.SetData(colors);
        }
        public static Texture2D TextTexture(string text, TextRenderOptions options = null)
        {
            options ??= new TextRenderOptions();
            using Font font = new Font(options.FontFamily, options.FontSize, options.FontStyle);

            Size textSize;
            using Bitmap tempBitmap = new Bitmap(1, 1);
            using Graphics tempGraphics = Graphics.FromImage(tempBitmap);
            textSize = Size.Round(tempGraphics.MeasureString(text, font));
            int width = textSize.Width + (options.Padding * 2);
            int height = textSize.Height;
            Bitmap bitmap = new Bitmap(width, height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (options.BackgroundColor != Microsoft.Xna.Framework.Color.Transparent)
            {
                using SolidBrush backgroundBrush = new SolidBrush(System.Drawing.Color.FromArgb(options.BackgroundColor.A, options.BackgroundColor.R, options.BackgroundColor.G, options.BackgroundColor.B));
                graphics.FillRectangle(backgroundBrush, 0, 0, width, height);

            }
            StringFormat stringFormat = new StringFormat
            {
                Alignment = options.HorizontalAlignment,
                LineAlignment = options.VerticalAlignment,
            };

            using SolidBrush textBrush = new SolidBrush(System.Drawing.Color.White);
            RectangleF rectF = new RectangleF(0, 0, width, height);
            graphics.DrawString(text, font, textBrush, rectF, stringFormat);

            using MemoryStream ms = new();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return Texture2D.FromStream(GraphicsDevice, ms);
        }
    }
    public class TextRenderOptions
    {
        public string FontFamily { get; set; } = "Arial";
        public float FontSize { get; set; } = 14;
        public Microsoft.Xna.Framework.Color BackgroundColor { get; set; } = Microsoft.Xna.Framework.Color.Transparent;
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;
        public int Padding { get; set; } = 5;
        public StringAlignment HorizontalAlignment { get; set; } = StringAlignment.Center;
        public StringAlignment VerticalAlignment { get; set; } = StringAlignment.Center;
    }
}
