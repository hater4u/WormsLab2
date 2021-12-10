using WormsLab.Interfaces;
using WormsLab.Actions;
using WormsLab.Utils;

namespace WormsLab.Models
{
    public class Worm: WorldObject
    {
        public const int ReproduceCost = 10;
        private static int _count = 0; 
        
        public readonly string Name;
        private IBehavior _behavior;
        private int _health;
        public int Health
        {
            get => _health;
            private set => _health = value;
        }
        
        public static int GlobalCount => _count;

        public bool IsDead => Health <= 0;

        public Worm(Point initialPosition, IBehavior behavior, string name = "John", int health = 10): base(initialPosition)
        {
            Name = name;
            _behavior = behavior;
            _health = health;
            _count++;
        }

        private void ChangePosition(Point delta)
        {
            Position += delta;
        }
        
        public void AddHealth(int delta)
        {
            Health += delta;
        }
        
        public void Reproduce()
        {
            Health -= ReproduceCost;
        }
        
        public void Tick()
        {
            Health--;
        }

        public IAction RequestNextAction(WorldSimulatorService worldSimulatorContext)
        {
            return _behavior.RequestNextAction(this, worldSimulatorContext);
        }
        
        public void TryApplyMoveInDirectionAction(MoveInDirectionAction moveDirection)
        {
            if (moveDirection != null)
                switch (moveDirection.Direction)
                {
                    case Direction.Up:
                        ChangePosition(new Point(0, 1));
                        break;
                    case Direction.Down:
                        ChangePosition(new Point(0, -1));
                        break;
                    case Direction.Right:
                        ChangePosition(new Point(1, 0));
                        break;
                    case Direction.Left:
                        ChangePosition(new Point(-1, 0));
                        break;
                }
        }
        
    }
}
