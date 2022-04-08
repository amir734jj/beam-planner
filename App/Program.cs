using System.IO;
using App.Interfaces;
using App.Logic;
using App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                .AddSingleton(_ => JsonConvert.DeserializeObject<BeamConfiguration>(File.ReadAllText("config.json")))
                .BuildServiceProvider();

            serviceProvider.GetRequiredService<IBeamPlanner>().Run(args[0]);
        }
    }
}