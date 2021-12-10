using WormsLab.Interfaces;
using WormsLab.Actions;
using WormsLab.Models;
using WormsLab.Utils;

namespace WormsLab.Behaviors
{
    public class ClockwiseMovementBehavior: IBehavior
    {
        private int _step = -1;
        
        private Direction[] _directions =  {
            Direction.Right,
            Direction.Down,
            Direction.Down,
            Direction.Left,
            Direction.Left,
            Direction.Up,
            Direction.Up,
            Direction.Right
        };
        
        public IAction RequestNextAction(Worm target, WorldSimulatorService world)
        {
            if (target.Position.x == 0 && target.Position.y == 0)
            {
                return new MoveInDirectionAction(Direction.Up);
            }

            _step = (_step + 1) % _directions.Length;

            return new MoveInDirectionAction(_directions[_step]);
        }

    }
}