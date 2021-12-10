using WormsLab.Utils;

namespace WormsLab.Models
{
    public class Food: WorldObject
    {
        private int _lifeTime;
        public int LifeTime => _lifeTime;

        public Food(Point initialPosition) : base(initialPosition)
        {
            _lifeTime = 10;
        }

        public void Tick()
        {
            _lifeTime--;
        }
    }
}