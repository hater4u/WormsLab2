using WormsLab.Interfaces;
using WormsLab.Utils;

namespace WormsLab.Actions
{
    public class MoveInDirectionAction: IAction
    {
        public Direction Direction;
        public ActionType GetActionType()
        {
            return ActionType.MoveInDirection;
        }

        public MoveInDirectionAction(Direction direction)
        {
            Direction = direction;
        }
    }
}