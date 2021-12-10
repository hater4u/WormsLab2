namespace WormsLab.Utils
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static float SqrDistance(Point a, Point b)
        {
            var deltaX = a.x - b.x;
            var deltaY = a.y - b.y;
            return deltaX * deltaX + deltaY * deltaY;
        }
        
        public static Point operator+(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }
        
        public static bool operator!=(Point a, Point b)
        {
            return !(a == b);
        }
        
        public static bool operator==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }
        
        public static Point operator-(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }
        
        public override bool Equals(object other)
        {
            if (!(other is Point))
            {
                return false;
            }

            return Equals((Point)other);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }
        
        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }
    }
}
