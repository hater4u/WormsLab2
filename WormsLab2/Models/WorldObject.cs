using WormsLab.Utils;

namespace WormsLab.Models
{
    public class WorldObject
    {
        public Point Position { get; protected set; }

        public WorldObject(Point initialPosition)
        {
            Position = initialPosition;
        }
    }
}