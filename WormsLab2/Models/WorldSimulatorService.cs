using Microsoft.Extensions.Hosting;
using WormsLab.Actions;
using WormsLab.Interfaces;
using WormsLab.Utils;

namespace WormsLab.Models
{
    public class WorldSimulatorService: IHostedService
    {
        private int _foodHealthRecover = 10;

        private List<Worm> _worms = new List<Worm>();
        private List<Food> _foods = new List<Food>();
        private IWriter _writer;
        private IFoodGenerator _foodGenerator;
        private IBehavior _behavior;
        private IWormNameGenerator _wormNameGenerator;

        public IReadOnlyCollection<Worm> Worms => _worms;
        public IReadOnlyCollection<Food> Foods => _foods;
        public WorldSimulatorService(IWriter writer, IFoodGenerator foodGenerator, IBehavior behavior, 
            IWormNameGenerator wormNameGenerator)
        {
            _writer = writer;
            _foodGenerator = foodGenerator;
            _behavior = behavior;
            _wormNameGenerator = wormNameGenerator;

            AddWorm(new Point(0, 0));
            WriteData();
        }
        
        private void AddWorm(Point position)
        {
            _worms.Add(new Worm(position, _behavior, _wormNameGenerator.GenerateName()));
        }

        public void Tick()
        {
            UpdateFood();
            UpdateWorms();
            TryEatFood();
            CheckForDeadWorms();
            WriteData();
        }
        
        private void UpdateFood()
        {
            foreach (var food in _foods)
            {
                food.Tick();
            }

            _foods.RemoveAll(i => i.LifeTime <= 0);

            GenerateFood();
        }
        
        private void GenerateFood()
        {
            Point position = _foodGenerator.GenerateFood(Foods);

            var worm = GetWormAt(position);

            if (worm != null)
            {
                worm.AddHealth(_foodHealthRecover);
                return;
            }

            _foods.Add(new Food(position));
        }
        
        private void UpdateWorms()
        {
            for (int i = 0; i < _worms.Count; i++)
            {
                _worms[i].Tick();
                var action = _worms[i].RequestNextAction(this);
                
                switch (action.GetActionType())
                {
                    case ActionType.Null:
                        break;
                    case ActionType.MoveInDirection:
                        ApplyMoveInDirection(_worms[i], action as MoveInDirectionAction);
                        break;
                    case ActionType.Reproduce:
                        ApplyReproduce(_worms[i], action as ReproduceAction);
                        break;
                }
            }
        }
        
        private void ApplyMoveInDirection(Worm target, MoveInDirectionAction? action)
        {
            if (action == null)
            {
                return;
            }

            var desiredPosition = target.Position + DirectionUtils.Direction2Point(action.Direction);
            if (GetWormAt(desiredPosition) == null)
            {
                target.TryApplyMoveInDirectionAction(action);
            }
        }
        
        private void ApplyReproduce(Worm target, ReproduceAction? action)
        {
            if (action == null)
            {
                return;
            }

            var desiredPosition = target.Position + DirectionUtils.Direction2Point(action.Direction);

            if (IsCellFree(desiredPosition) && target.Health > Worm.ReproduceCost)
            {
                AddWorm(desiredPosition);
            }

            target.Reproduce();
        }
        
        private void TryEatFood()
        {
            foreach (var worm in _worms)
            {
                var food = GetFoodAt(worm.Position);

                if (food != null)
                {
                    _foods.Remove(food);
                    worm.AddHealth(_foodHealthRecover);
                }
            }
        }
        
        private void CheckForDeadWorms()
        {
            _worms.RemoveAll(i => i.IsDead);
        }

        private bool IsCellFree(Point cell)
        {
            return GetObjectAt(cell) == null;
        }

        private WorldObject? GetObjectAt(Point position)
        {
            var worm = GetWormAt(position);

            if (worm != null)
            {
                return worm;
            }

            var food = GetFoodAt(position);

            if (food != null)
            {
                return food;
            }

            return null;
        }

        private Worm? GetWormAt(Point position)
        {
            foreach (var worm in _worms)
            {
                if (worm.Position == position)
                {
                    return worm;
                }
            }

            return null;
        }

        public Food? GetFoodAt(Point position)
        {
            foreach (var food in _foods)
            {
                if (food.Position == position)
                {
                    return food;
                }
            }

            return null;
        }

        private void WriteData()
        {
            var message = "Worms:[";
            var first = true;
            
            foreach (var worm in _worms)
            {
                if(!first)
                {
                    message += ",";
                }
                message += $"{worm.Name}-{worm.Health}({worm.Position.x},{worm.Position.y})";
                first = false;
            }
            
            message += "], Food:[";
            first = true;
            
            foreach (var food in _foods)
            {
                if (!first)
                {
                    message += ",";
                }
                message += $"({food.Position.x},{food.Position.y})";
                first = false;
            }

            message += "]";
            _writer.WriteLine(message);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                Tick();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}