using System;

namespace Stitcher.Core
{
    public class Rect
    {
        public int X1 { get; private set; }
        public int Y1 { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Rect(int x, int y, int width, int height)
        {
            X1 = x;
            Y1 = y;
            Width = width;
            Height = height;
        }

        public Rect Clone()
        {
            return new Rect(X1, Y1, Width, Height);
        }

        public void Translate(int deltaX, int deltaY)
        {
            X1 += deltaX;
            Y1 += deltaY;
        }

        public bool InBounds(int x, int y)
        {
            return x >= X1 && y >= Y1 && x < X2 && y < Y2;
        }

        public int X2 => X1 + Width;
        public int Y2 => Y1 + Height;
    }
}
