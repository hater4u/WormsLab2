using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WormsLab.Actions;
using WormsLab.Behaviors;
using WormsLab.Generators;
using WormsLab.Interfaces;
using WormsLab.Models;
using WormsLab.Utils;
using WormsLab.Writers;

namespace WormLabTests;

public class Tests
{
    [Test]
    public void EmptyCellMovement()
    {
        WorldSimulatorService worldSimulator = new WorldSimulatorService(
            new ConsoleWriter(),
            new SimpleFoodGenerator(),
            new ClockwiseMovementBehavior(),
            new SimpleWormNameGenerator()
            );

        // First worm starts on (0, 0)
        Assert.AreEqual(worldSimulator.Worms.Count, 1);
        Assert.AreEqual(worldSimulator.Worms.ElementAt(0).Position, new Point(0, 0));

        // And moves clockwise
        worldSimulator.Tick();
        Assert.AreEqual(worldSimulator.Worms.ElementAt(0).Position, new Point(0, 1));

        worldSimulator.Tick();
        Assert.AreEqual(worldSimulator.Worms.ElementAt(0).Position, new Point(1, 1));
    }

    [Test]
    public void EatFood()
    {
        WorldSimulatorService worldSimulator = new WorldSimulatorService(
            new ConsoleWriter(),
            new TestFoodSpawner(new Point(1, 1)),
            new ClockwiseMovementBehavior(),
            new SimpleWormNameGenerator()
            );

        // Food generates on (1, 1) so it must be eaten on 2nd tick
        worldSimulator.Tick();
        worldSimulator.Tick();
        Assert.AreEqual(worldSimulator.Foods.Count, 1);
        Assert.Greater(worldSimulator.Worms.ElementAt(0).Health, 10);
    }

    public class TestFoodSpawner : IFoodGenerator
    {
        private Point _targetFoodPosition;

        public TestFoodSpawner(Point initialPostion)
        {
            _targetFoodPosition = initialPostion;
        }

        public Point GenerateFood(IReadOnlyCollection<WorldObject> forbiddenCells)
        {
            while (Contains(forbiddenCells, _targetFoodPosition))
            {
                _targetFoodPosition += new Point(0, 1);
            }

            var result = _targetFoodPosition;

            _targetFoodPosition += new Point(0, 1);

            return result;
        }

        public bool Contains(IReadOnlyCollection<WorldObject> collection, Point position)
        {
            foreach (var obj in collection)
            {
                if (obj.Position == position)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Test]
    public void FoodSpawnOnWorm()
    {
        WorldSimulatorService worldSimulator = new WorldSimulatorService(
            new ConsoleWriter(),
            new TestFoodSpawner(new Point(0, 0)),
            new TestWormAI(0),
            new SimpleWormNameGenerator()
            );


        worldSimulator.Tick();
        Assert.AreEqual(worldSimulator.Foods.Count, 0);
        Assert.Greater(worldSimulator.Worms.ElementAt(0).Health, 10);
    }

    [Test]
    public void ReproduceInBusyCell()
    {
        WorldSimulatorService worldSimulator = new WorldSimulatorService(
            new ConsoleWriter(),
            new TestFoodSpawner(new Point(0, 0)),
            new TestWormAI(0),
            new SimpleWormNameGenerator()
            );

        worldSimulator.Tick();
        worldSimulator.Tick();

        Assert.AreEqual(worldSimulator.Worms.Count, 1);
        // 10 hp spend regardless of reproduce success
        Assert.Less(worldSimulator.Worms.ElementAt(0).Health, 10);
    }

    [Test]
    public void ReproduceInEmptyCell()
    {
        WorldSimulatorService worldSimulator = new WorldSimulatorService(
            new ConsoleWriter(),
            new TestFoodSpawner(new Point(0, 0)),
            new TestWormAI(1),
            new SimpleWormNameGenerator()
            );

        worldSimulator.Tick();
        worldSimulator.Tick();

        Assert.AreEqual(worldSimulator.Worms.Count, 2);
        Assert.AreNotEqual(worldSimulator.Worms.ElementAt(0).Name, worldSimulator.Worms.ElementAt(1).Name);
        Assert.Less(worldSimulator.Worms.ElementAt(0).Health, 10);
    }

    [Test]
    public void MoveOnWorm()
    {
        WorldSimulatorService worldSimulator = new WorldSimulatorService(
            new ConsoleWriter(),
            new TestFoodSpawner(new Point(0, 0)),
            new TestWormAI(1),
            new SimpleWormNameGenerator()
            );

        worldSimulator.Tick();
        worldSimulator.Tick();

        var wormPosition = worldSimulator.Worms.ElementAt(0).Position;

        worldSimulator.Tick();

        var newWormPosition = worldSimulator.Worms.ElementAt(0).Position;

        Assert.AreEqual(wormPosition, newWormPosition);
    }

    public class TestWormAI : IBehavior
    {
        private bool _firstMove = true;
        private int _currentMove = 0;
        private bool _mustReproduce = true;
        private Direction _lastReproduceDirection;

        public TestWormAI(int initialMove)
        {
            _currentMove = initialMove;
        }

        public IAction RequestNextAction(Worm target, WorldSimulatorService world)
        {
            if (_firstMove)
            {
                _firstMove = false;
                return new NullAction();
            }

            if (_mustReproduce)
            {
                _mustReproduce = !_mustReproduce;
                var values = Enum.GetValues(typeof(Direction));
                var direciton = (Direction)values.GetValue(_currentMove % values.Length);
                _currentMove++;
                _lastReproduceDirection = direciton;
                return new ReproduceAction(direciton);
            }
            else
            {
                _mustReproduce = !_mustReproduce;
                Console.WriteLine("Trying to move...");
                return new MoveInDirectionAction(_lastReproduceDirection);
            }
        }
    }

    [Test]
    public void ClosestFoodMovement()
    {
        WorldSimulatorService worldSimulator = new WorldSimulatorService(
            new ConsoleWriter(),
            new SimpleFoodGenerator(1),
            new ChaseClosestFood(),
            new SimpleWormNameGenerator()
            );

        for (int i = 0; i < 100; i++)
        {
            var oldWormPosition = worldSimulator.Worms.ElementAt(0).Position;
            var oldHealth = worldSimulator.Worms.ElementAt(0).Health;

            worldSimulator.Tick();

            if(!worldSimulator.Foods.Any())
            {
                continue;
            }

            if(oldHealth < worldSimulator.Worms.ElementAt(0).Health)
            {
                continue;
            }

            var newWormPosition = worldSimulator.Worms.ElementAt(0).Position;
            var foods = worldSimulator.Foods;
            var minDistance = int.MaxValue;

            foreach(var food in foods)
            {
                minDistance = Math.Min(minDistance, GetDiscreteDistance(oldWormPosition, food.Position));
            }

            var closestFoods = new List<Food>();

            foreach (var food in foods)
            {
                var distance = GetDiscreteDistance(oldWormPosition, food.Position);

                if(distance == minDistance)
                {
                    closestFoods.Add(food);
                }
            }

            bool success = false;

            foreach (var food in closestFoods)
            {
                if(GetDiscreteDistance(newWormPosition, food.Position) < minDistance)
                {
                    success = true;
                    break;
                }
            }

            if(!success)
            {
                Assert.Fail();
            }

        }

        int GetDiscreteDistance(Point a, Point b)
        {
            var direction = b - a;
            return Math.Abs(direction.x) + Math.Abs(direction.y);
        }
    }

    [Test]
    public void UniqueNameGeneration()
    {
        IWormNameGenerator generator = new SimpleWormNameGenerator(10);
        List<String> names = new List<string>();
        
        for (int i = 0; i < 10; i++)
        {
            names.Add(generator.GenerateName());
        }

        foreach (var name in names)
        {
            var len = names.FindAll(x => x == name).Count;
            Assert.AreEqual(1, len);
        }
        
    }
}