using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ui.Services.Overlay.Form
{
    public class ResizeCorner : Gadget
    {

        Point resizeStartPos = new Point(-1, -1);
        Point resizeStartSize = new Point(-1, -1);
        VertexPositionColor[] cornerVertexs = new VertexPositionColor[3];
        Color color = Color.Blue;
        static BasicEffect effect;
        static VertexBuffer vertexBuffer;
        public bool Resizing = false;
        static ResizeCorner()
        {
            effect = new BasicEffect(ReadHelper.Instance.GraphicsDevice);
            effect.VertexColorEnabled = true;
           vertexBuffer = new VertexBuffer(ReadHelper.Instance.GraphicsDevice, VertexPositionColor.VertexDeclaration, 3, BufferUsage.WriteOnly);
        }
        void setVertexs()
        {
            cornerVertexs[0] = new VertexPositionColor(new Vector3(Rect.X, 0, 0), color);
            cornerVertexs[1] = new VertexPositionColor(new Vector3(Rect.X, 1, 0), color);
            cornerVertexs[2] = new VertexPositionColor(new Vector3(0, Rect.Height, 0), color);
            vertexBuffer.SetData(cornerVertexs);
        }
        void mouseInHandler(object sender, InputEventArgs e)
        {
            color = new Color(30, 30, 30, 0);
        }
        void mouseOutHandler(object sender, InputEventArgs e)
        {
            color = Color.Blue;
        }
        void mousePressHandler(object sender, InputEventArgs e)
        {
            Resizing = true;
        }
        void mouseReleaseHandler(object sender, InputEventArgs e)
        {
            Resizing = false;
            resizeStartPos = new Point(-1, -1);
        }
        void mouseMoveHandler(MouseState mouse)
        {
            if (!Resizing) return;
            if (resizeStartPos.X < 0)
            {
                resizeStartPos = mouse.Position;
                resizeStartSize = new Point(Parent.Rect.Width, Parent.Rect.Height);
            }
            int width = mouse.Position.X - resizeStartPos.X + resizeStartSize.X;
            int height = mouse.Position.Y - resizeStartPos.Y + resizeStartSize.Y;
            if (width <= 100) width = 100;
            if (height <= 100) height = 100;
            Parent.Rect = new Rectangle(Parent.Rect.X, Parent.Rect.Y, width, height);
        }
        void checkResizingForm()
        {
            if (MouseIn) return;
            foreach (var form in VirtualForm.AllForms)
            {
                if (Equals(form, Parent) || !form.Resizing) continue;
                Resizing = false;
            }
        }
        public override void Load()
        {
            Rect = new Rectangle(0, 0, 20, 20);
            OnMouseIn += mouseInHandler;
            OnMouseOut += mouseOutHandler;
            OnLeftMouseBtnPress += mousePressHandler;
            OnLeftMouseBtnRelease += mouseReleaseHandler;
            base.Load();
        }
        public override void Update(GameTime gametime, MouseState mouse)
        {
            RelativePosition = new Point(Parent.Rect.Width - Rect.Width, Parent.Rect.Height - Rect.Height);
            checkResizingForm();
            mouseMoveHandler(mouse);

            base.Update(gametime, mouse);
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Overlay overlay)
        {

            setVertexs();
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, vertexBuffer.VertexCount);
            }
        }
    }
}
