using WormsLab.Models;
using WormsLab.Utils;

namespace WormsLab.Interfaces;

public interface IFoodGenerator
{
    Point GenerateFood(IReadOnlyCollection<WorldObject> forbiddenCells);
}