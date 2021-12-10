using WormsLab.Models;

namespace WormsLab.Interfaces
{
    public interface IBehavior
    {
        public IAction RequestNextAction(Worm target, WorldSimulatorService world);
    }
}