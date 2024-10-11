using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ui
{
    public class Texture
    {

        public Texture2D PixelTexture;
        public Texture2D CornerTexture;
        public Texture(GraphicsDevice graphicsDevice)
        {
            PixelTexture = new Texture2D(graphicsDevice, 1, 1);
            PixelTexture.SetData(new[] { Color.White });


            CornerTexture = new Texture2D(graphicsDevice, 100, 100);
            Color[] colors = new Color[CornerTexture.Width * CornerTexture.Height];
            int fillCol = CornerTexture.Width - 1;
            for (int i = 0; i < CornerTexture.Width; i++)
            {
                for (int ii = 0; ii < CornerTexture.Height; ii++)
                {
                    if (ii < fillCol) continue;
                    colors[i * CornerTexture.Width + ii] = Color.White;
                }
                fillCol--;
            }
            CornerTexture.SetData(colors);
        }
    }
}
