﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.Xna.Framework.Input;

namespace ReadHelper.Services.Overlay.Form
{
    public class VirtualForm : ParentGadget
    {

        public static readonly HashSet<VirtualForm> AllForms = [];
        Color bg = new(33, 33, 33);
        readonly FormHead head;
        readonly ResizeCorner resizeCorner;
        public bool Moving { get => head.Moving; }
        public bool Resizing { get => resizeCorner.Resizing; }
        public bool Resizeable { get => !resizeCorner.Disabled; set => resizeCorner.Disabled = !value; }
        public VirtualForm()
        {
            AllForms.Add(this);
            head = new FormHead() { Parent = this };
            resizeCorner = new ResizeCorner() { Parent = this, Disabled = true };
            OnLeftMouseBtnPress += ToTop;
            OnRectChange += HandleRectChange;

        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = Rect;
            spriteBatch.Begin(rasterizerState: new RasterizerState() { ScissorTestEnable = true });
            spriteBatch.Draw(ReadHelper.Texture.PixelTexture, Rect, bg);
            base.Draw(spriteBatch, overlay); // draw children
        }
        private void ToTop(object sender, MouseEventArgs e)
        {
            Gadget topGadget = Parent.Children.MaxBy(child => child.ZIndex);
            if (Equals(topGadget, this)) return;
            ZIndex = topGadget.ZIndex + 1;
        }

        private void HandleRectChange(object sender, ChangeEvent<Rectangle> e)
        {
            Point diffPoint = e.Current.Location - e.Old.Location;
            foreach (var child in Children)
            {
                if (Equals(child, head)) continue;
                child.Rect = new(new Point(child.Rect.X + diffPoint.X, child.Rect.Y + diffPoint.Y), child.Rect.Size);
            }
        }
    }
}
