using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public struct Point
    {
        public Point(int X, int Y)
        {
            x = X; y = Y;
        }
        public int x;
        public int y;

        public static Point operator +(Point origin, Point offset)
        {
            return new Point(origin.x + offset.x, origin.y + offset.y);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }
    }
}
