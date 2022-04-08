using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using App.Interfaces;
using App.Models;
using App.Models.Instructions;
using Microsoft.Extensions.Logging;

namespace App.Logic
{
    public class BeamPlanner : IBeamPlanner
    {
        private readonly ILogger<BeamPlanner> _logger;

        private const double MaxSimultaneousBeams = 32;

        private const double UserAngle = 45;

        private const int MaxInterferesInterference = 20;
        
        private const int MaxSelfInterference = 20;

        private const int NumColors = 4;
        
        public BeamPlanner(ILogger<BeamPlanner> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Calculate angle between 3 points in 3D space.
        /// Note: assumes we want 1 vector to run from coord1 -> coord2, and the other from coord3 -> coord2.
        /// </summary>
        /// <param name="a">coordinate a.</param>
        /// <param name="b">coordinate b.</param>
        /// <param name="c">coordinate c.</param>
        /// <returns>Inner angle between 3 coordinates.</returns>
        private double CalculateAlpha(Coordinate a, Coordinate b, Coordinate c)
        {
            var ba = Normalize(new Coordinate(a.X - b.X, a.Y - b.Y, a.Z - b.Z));
            var bc = Normalize(new Coordinate(c.X - b.X, c.Y - b.Y, c.Z - b.Z));

            // Dot product
            var dotProduct = ba.X * bc.X + ba.Y * bc.Y + ba.Z * bc.Z;

            // Extract the angle from the dot products
            var angle = Math.Acos(dotProduct) * 180.0 / Math.PI;

            return angle;
        }

        private IEnumerable<OrderInstruction> Analyzer(Subroutine subroutine)
        {
            var users = subroutine.Instructions.Where(x => x is UserInstruction)
                .Cast<UserInstruction>()
                .ToList();

            var satellites = subroutine.Instructions.Where(x => x is SatelliteInstruction)
                .Cast<SatelliteInstruction>()
                .ToList();

            var interferes = subroutine.Instructions.Where(x => x is InterfererInstruction)
                .Cast<InterfererInstruction>()
                .ToList();

            // Map of user with list of all available satellites
            var userSatelliteVisibilityMap = users.SelectMany(user => satellites
                    // User angle should be <= 45 degrees
                    .Where(satellite =>
                        CalculateAlpha(user.Coordinate, Coordinate.Origin, satellite.Coordinate) +  UserAngle > 180.0)
                    .Where(satellite =>
                        // Satellites must not be within 20 degrees of a beam from interferes
                        interferes
                            .All(interfere =>
                                CalculateAlpha(user.Coordinate, interfere.Coordinate, satellite.Coordinate) >= MaxInterferesInterference))
                    .Select(satellite => (user, satellite)))
                .GroupBy(t => t.user)
                .ToDictionary(t => t.Key, t => t.Select(p => p.satellite).ToList());
            
            var satelliteAvailability = satellites.ToDictionary(satellite => satellite, _ => 0);
            var satelliteColors = satellites
                .ToDictionary(satellite => satellite, _ => Enumerable.Range(0, NumColors)
                    .ToDictionary(x => x, _ => false));
            var satelliteBeams = satellites
                .ToDictionary(satellite => satellite, _ => new List<Coordinate>());
            
            foreach (var (user, availableSatellites) in userSatelliteVisibilityMap)
            {
                foreach (var satellite in availableSatellites)
                {
                    // Up to 32 beams simultaneously
                    if (satelliteAvailability[satellite] < MaxSimultaneousBeams)
                    {
                        satelliteAvailability[satellite]++;

                        foreach (var color in Enumerable.Range(0, NumColors))
                        {
                            if (satelliteBeams[satellite].All(beam => CalculateAlpha(satellite.Coordinate, user.Coordinate, beam) >= MaxSelfInterference))
                            {
                                satelliteBeams[satellite].Add(user.Coordinate);
                                
                                yield return new OrderInstruction(satellite.Id, satelliteBeams[satellite].Count, user.Id, color);
                            }
                        }
                    }
                }
            }
        }

        private double Magnitude(Coordinate coordinate)
        {
            return Math.Sqrt(Math.Pow(coordinate.X, 2) + Math.Pow(coordinate.Y, 2) + Math.Pow(coordinate.Z, 2));
        }

        private Coordinate Normalize(Coordinate coordinate)
        {
            var magnitude = Magnitude(coordinate);

            return new Coordinate(coordinate.X / magnitude, coordinate.Y / magnitude, coordinate.Z / magnitude);
        }

        public void Run(string filename)
        {
            var subroutine = Parser(filename);

            var result = Analyzer(subroutine).ToList();

            var text = string.Join(Environment.NewLine, result);

            Console.WriteLine(text);
        }

        /// <summary>
        /// Utility function that parses input given the CFG.
        /// </summary>
        /// <param name="filename">file path.</param>
        /// <returns><see cref="Subroutine"/> AST node.</returns>
        private Subroutine Parser(string filename)
        {
            var str = CharStreams.fromPath(filename);

            var lexer = new BeamLexer(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new BeamParser(tokens);

            var listenerLexer = new ConsoleErrorListener<int>();
            var listenerParser = new ConsoleErrorListener<IToken>();

            lexer.AddErrorListener(listenerLexer);
            parser.AddErrorListener(listenerParser);

            foreach (var token in lexer.GetAllTokens())
            {
                if (token.Channel == Lexer.DefaultTokenChannel)
                {
                    _logger.LogTrace("{%s}: {%s}", lexer.Vocabulary.GetSymbolicName(token.Type), token.Text);
                }
            }

            lexer.Reset();

            var tree = parser.subroutine();
            var visitor = new BeamVisitor();

            var subroutine = (Subroutine)visitor.Visit(tree);

            return subroutine;
        }
    }
}