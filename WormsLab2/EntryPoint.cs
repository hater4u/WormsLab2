using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WormsLab.Behaviors;
using WormsLab.Generators;
using WormsLab.Interfaces;
using WormsLab.Models;
using WormsLab.Writers;

namespace WormsLab2
{
    public class EntryPoint
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WorldSimulatorService>();
                    services.AddScoped<IWriter>(ctx => new FileWriter("output.txt"));
                    services.AddScoped<IFoodGenerator>(ctx => new SimpleFoodGenerator(1));
                    services.AddScoped<IBehavior>(ctx => new ChaseClosestFood());
                    services.AddScoped<IWormNameGenerator>(ctx => new SimpleWormNameGenerator(42));
                });
        }
    }
}