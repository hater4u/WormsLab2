using WormsLab.Interfaces;
using WormsLab.Models;
using WormsLab.Utils;

namespace WormsLab.Generators;

public class SimpleFoodGenerator: IFoodGenerator
{
    private Random _random;

    public SimpleFoodGenerator(int? seed = null)
    {
        _random = seed == null ? new Random() : new Random((int)seed);
    }

    public Point GenerateFood(IReadOnlyCollection<WorldObject> forbiddenCells)
    {
        Point position;
        do
        {
            position = new Point(_random.NextNormal(0, 5), _random.NextNormal(0, 5));

        } while (!IsCellFree(position, forbiddenCells));
        return position;
    }

    public bool IsCellFree(Point position, IReadOnlyCollection<WorldObject> forbiddenCells)
    {
        foreach (var cell in forbiddenCells)
        {
            if(position == cell.Position)
            {
                return false;
            }
        }

        return true;
    }
}