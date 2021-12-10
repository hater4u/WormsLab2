using WormsLab.Actions;
using WormsLab.Interfaces;
using WormsLab.Models;
using WormsLab.Utils;

namespace WormsLab.Behaviors;

public class ChaseClosestFood: IBehavior
{
    public IAction RequestNextAction(Worm target, WorldSimulatorService world)
    {
        if(world.Foods.Count == 0)
        {
            return new NullAction();
        }

        var closest = FindClosestFood(target.Position, world);

        var direction = closest - target.Position;

        if(Math.Abs(direction.x) > Math.Abs(direction.y))
        {
            return new MoveInDirectionAction(DirectionUtils.Point2Direction(new Point(Math.Sign(direction.x), 0)));
        }

        return new MoveInDirectionAction(DirectionUtils.Point2Direction(new Point(0, Math.Sign(direction.y))));
    }

    private Point FindClosestFood(Point wormPosition, WorldSimulatorService world)
    {
        var minDistance = int.MaxValue;
        var closest = wormPosition;

        foreach (var food in world.Foods)
        {
            var distance = GetDiscreteDistance(wormPosition, food.Position);
            if(distance < minDistance)
            {
                minDistance = distance;
                closest = food.Position;
            }
        }
        
        int GetDiscreteDistance(Point a, Point b)
        {
            var direction = b - a;
            return Math.Abs(direction.x) + Math.Abs(direction.y);
        }
        
        return closest;
    }
}