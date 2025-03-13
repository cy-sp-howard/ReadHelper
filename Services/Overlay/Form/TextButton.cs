using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ReadHelper.WindowUtil;

namespace ReadHelper.Services.Overlay.Form
{

    public class TextButton : Gadget
    {
        readonly Texture2D textTexture;
        public Color bg
        {
            get => _bg; set
            {
                _bg = value;
                currentBg = value;
            }
        }
        Color _bg = new(47, 47, 47);
        Color color = Color.White;
        Color currentBg = new(47, 47, 47);
        Point padding = new(5, 3);
        int TextWidth
        {
            get => textTexture.Width / 2;
        }
        int TextHeight
        {
            get => textTexture.Height / 2;
        }
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                UpdateSize();
            }

        }
        int _width;
        public TextButton(string text, Gadget parent) : base(parent)
        {
            textTexture = Texture.TextTexture(text, new() { FontSize = 24 });
            UpdateSize();
            RelativePosition = new Point((Parent.Size.X - Size.X) / 2, (Parent.Size.Y - Size.Y) / 2);
            OnMouseIn += MouseInHandler;
            OnMouseOut += MouseOutHandler;
        }
        void MouseInHandler(object sender, MouseEventArgs e)
        {
            currentBg = Color.DarkSlateGray;
        }
        void MouseOutHandler(object sender, MouseEventArgs e)
        {
            currentBg = bg;
        }
        void UpdateSize()
        {
            int textWrapperWidth = TextWidth + padding.X * 2;
            int textWrapperHeight = TextHeight + padding.Y * 2;
            Size = new Point(Width > 0 ? Width : textWrapperWidth, textWrapperHeight);
        }
        public override void DrawInRect(SpriteBatch spriteBatch, OverlayRoot overlay)
        {
            spriteBatch.Draw(Texture.PixelTexture, Rect, currentBg);
            spriteBatch.Draw(textTexture, new Rectangle(Rect.X + padding.X, Rect.Y + padding.Y, TextWidth, TextHeight), color);

        }
    }
}
