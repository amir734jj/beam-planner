using System;
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
                .AddSingleton(_ =>
                    JsonConvert.DeserializeObject<BeamConfiguration>(
                        File.ReadAllText(Path.Join(Environment.GetEnvironmentVariable("CONFIG_PATH"), "config.json"))))
                .BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            if (args.Length != 1)
            {
                logger.LogError("Input path should be provided as argument");
            }
            else if (!File.Exists(args[0]))
            {
                logger.LogError("File path does not exist");
            }
            else
            {
                serviceProvider.GetRequiredService<IBeamPlanner>().Run(args[0]);
            }
        }
    }
}