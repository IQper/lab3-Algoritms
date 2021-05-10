using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace K_means
{
    struct Dot
    {
        public int X;
        public int Y;
        public Color Color;

        public Dot(int x, int y)
        {
            X = x;
            Y = y;
            Color = Color.Gray;
        }

        public Dot(int x, int y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public double DistanceTo(Dot dot)
        {
            return Math.Sqrt(Math.Pow(dot.X - X, 2) + Math.Pow(dot.Y - Y, 2));
        }

        public static Dot operator +(Dot dot1, Dot dot2)
        {
            return new Dot(dot1.X + dot2.X, dot1.Y + dot2.Y, dot1.Color);
        }

        public static Dot operator /(Dot dot1, int x)
        {
            return new Dot(dot1.X / x, dot1.Y / x, dot1.Color);
        }
    }
}
