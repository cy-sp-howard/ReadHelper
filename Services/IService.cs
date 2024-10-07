using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ui.Services
{
    internal interface IService
    {
        void Load();
        void Update(GameTime gametime);
        void Draw(SpriteBatch spriteBatch);
    }
}
