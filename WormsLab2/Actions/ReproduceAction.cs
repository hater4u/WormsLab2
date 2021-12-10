using WormsLab.Interfaces;
using WormsLab.Utils;

namespace WormsLab.Actions;

public class ReproduceAction: IAction
{
    public Direction Direction { get; }

    public ActionType GetActionType()
    {
        return ActionType.Reproduce;
    }
    
    public ReproduceAction(Direction direction)
    {
        Direction = direction;
    }
}