using WormsLab.Interfaces;
using WormsLab.Utils;

namespace WormsLab.Actions
{
    public class NullAction: IAction
    {
        public ActionType GetActionType()
        {
            return ActionType.Null;
        }
    }
}