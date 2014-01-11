using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI_Test_Tool.Enums;

namespace UI_Test_Tool.Models
{
    public class Element
    {
        // position
        // rectangle
        // offsetreference - topcenter - absolutecenter - bottomcenter - leftcenter - rightcenter - bottomleft - bottomright - topleft - topright

        public Rectangle baseRect { get; set; }
        public Rectangle Rect { get; set; }
        //public Point Position { get; set; }

        public Element(Rectangle baseRect, Rectangle rect)
        {
            this.baseRect = baseRect;
            this.Rect = rect;
            //this.Position = position;
        }

        public void MouseClick(MouseClickType type, OffsetReference offset)
        {
            if (Rect != null)
            {
                Point clickPos = new Point();
                clickPos.X = baseRect.X;
                clickPos.Y = baseRect.Y;

                switch (offset)
                {
                    case OffsetReference.AbsoluteCenter:
                        clickPos.Y += Rect.Y + (Rect.Height / 2);
                        clickPos.X += Rect.X + (Rect.Width / 2); 
                        break;
                    case OffsetReference.BottomCenter:
                        clickPos.Y += Rect.Y + Rect.Height;
                        clickPos.X += Rect.X + (Rect.Width / 2);
                        break;
                    case OffsetReference.TopCenter:
                        clickPos.Y += Rect.Y;
                        clickPos.X += Rect.X + (Rect.Width / 2);
                        break;
                    case OffsetReference.RightCenter:
                        clickPos.Y += Rect.Y + (Rect.Height / 2);
                        clickPos.X += Rect.X + Rect.Width;
                        break;
                    case OffsetReference.LeftCenter:
                        clickPos.Y += Rect.Y + (Rect.Height / 2);
                        clickPos.X += Rect.X;
                        break;
                    case OffsetReference.BottomLeft:
                        clickPos.Y += Rect.Y + Rect.Height;
                        clickPos.X += Rect.X;
                        break;
                    case OffsetReference.BottomRight:
                        clickPos.Y += Rect.Y + Rect.Height;
                        clickPos.X += Rect.X + Rect.Width;
                        break;
                    case OffsetReference.TopLeft:
                        clickPos.Y += Rect.Y;
                        clickPos.X += Rect.X;
                        break;
                    case OffsetReference.TopRight:
                        clickPos.Y += Rect.Y;
                        clickPos.X += Rect.X + Rect.Width;
                        break;
                }

                switch (type)
                {
                    case MouseClickType.LeftClick:
                        MouseOperations.SetCursorPosition(clickPos.X, clickPos.Y);
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                        break;
                    case MouseClickType.RightClick:
                        MouseOperations.SetCursorPosition(clickPos.X, clickPos.Y);
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                        break;
                }
            }

            else
            {
                //log error
            }
        }
    }
}
