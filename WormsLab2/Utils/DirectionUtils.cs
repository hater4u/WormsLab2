namespace WormsLab.Utils;

public static class DirectionUtils
{
    public static Direction Point2Direction(Point direction)
    {
        if(direction == new Point(0 , 1))
        {
            return Direction.Up;
        }

        if(direction == new Point(0 , -1))
        {
            return Direction.Down;
        }

        if(direction == new Point(1 , 0))
        {
            return Direction.Right;
        }

        if(direction == new Point(-1 , 0))
        {
            return Direction.Left;
        }

        throw new ArgumentException("Direction must be unit vector!");
    }

    public static Point Direction2Point(Direction direction)
    {
        if(direction == Direction.Up)
        {
            return new Point(0, 1);
        }

        if(direction == Direction.Down)
        {
            return new Point(0, -1);
        }

        if(direction == Direction.Right)
        {
            return new Point(1, 0);
        }

        if(direction == Direction.Left)
        {
            return new Point(-1, 0);
        }

        throw new ArgumentException("Direction must be unit vector!");
    }
}