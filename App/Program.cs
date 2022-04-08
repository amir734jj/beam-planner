using App.Interfaces;
using App.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(cfg => cfg.AddConsole())
                .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Error)
                .AddSingleton<IBeamPlanner, BeamPlanner>()
                .BuildServiceProvider();

            serviceProvider.GetRequiredService<IBeamPlanner>().Run(args[0]);
        }
    }
}