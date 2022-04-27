using System;
using System.Collections.Concurrent;
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
        private readonly BeamConfiguration _beamConfiguration;

        private readonly ILogger<BeamPlanner> _logger;

        public BeamPlanner(BeamConfiguration beamConfiguration, ILogger<BeamPlanner> logger)
        {
            _beamConfiguration = beamConfiguration;
            _logger = logger;
        }

        private IEnumerable<OrderInstruction> Analyzer(Subroutine subroutine)
        {
            // Collect users
            var users = subroutine.Instructions.Where(x => x is UserInstruction)
                .Cast<UserInstruction>()
                .ToList();

            // Collect satellites
            var satellites = subroutine.Instructions.Where(x => x is SatelliteInstruction)
                .Cast<SatelliteInstruction>()
                .ToList();

            // Collect interferes
            var interferes = subroutine.Instructions.Where(x => x is InterfererInstruction)
                .Cast<InterfererInstruction>()
                .ToList();
            
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None
            };

            // Map of satellite with list of all potential users
            var satelliteUserVisibilityMap = users
                // Parallelize the loop over users
                .AsParallel()
                .SelectMany(user => satellites
                    // User angle should be <= 45 degrees
                    .Where(satellite =>
                        CalculateAlpha(user.Coordinate, Coordinate.Origin, satellite.Coordinate) +
                        _beamConfiguration.UserAngle > 180.0)
                    .Where(satellite =>
                        // Satellites must not be within 20 degrees of a beam from interferes
                        interferes
                            .All(interfere =>
                                CalculateAlpha(user.Coordinate, interfere.Coordinate, satellite.Coordinate) >=
                                _beamConfiguration.MaxInterferesInterference))
                    .Select(satellite => (satellite, user)))
                .GroupBy(t => t.satellite)
                .ToDictionary(t => t.Key, t => t.Select(p => p.user).ToList());

            // Map of satellites with current count of utilization
            var satelliteAvailability = satellites.ToDictionary(satellite => satellite, _ => 0);
            // Map of satellites with current utilization with current count of utilization mapped by colors
            var satelliteBeams = satellites
                .ToDictionary(satellite => satellite,
                    _ => Enumerable.Range(0, _beamConfiguration.NumColors)
                        .ToDictionary(color => color, _ => new List<Coordinate>()));

            // Map of user assignments
            var userAssignments = users.ToDictionary(user => user, _ => false);

            // Service highest population of potential users first
            foreach (var (satellite, potentialUsers) in
                     satelliteUserVisibilityMap
                          // Parallelize the loop over satellite/users map
                          .OrderBy(x => x.Value.Count))
            {
                // Assign beam to users that have not been assigned yet
                foreach (var user in potentialUsers.Where(user => !userAssignments[user]))
                {
                    // Satellite is fully utilized and cannot service any more user
                    if (satelliteAvailability[satellite] >= _beamConfiguration.MaxSimultaneousBeams)
                    {
                        break;
                    }

                    foreach (var color in Enumerable.Range(0, _beamConfiguration.NumColors))
                    {
                        // On each Starlink satellite, no two beams of the same color may be pointed within 10 degrees
                        // of each other, or they will interfere with each other.
                        if (satelliteBeams[satellite][color].All(beam =>
                                CalculateAlpha(satellite.Coordinate, user.Coordinate, beam) >=
                                _beamConfiguration.MaxSelfInterference))
                        {
                            // Satellite is now supporting one more user
                            satelliteAvailability[satellite]++;
                            // Satellite at this color is now supporting user with this coordinate
                            satelliteBeams[satellite][color].Add(user.Coordinate);
                            // Mark this user as served
                            userAssignments[user] = true;

                            yield return new OrderInstruction(
                                satellite.Id,
                                // Get count of list of users this satellite is currently supporting 
                                satelliteAvailability[satellite],
                                user.Id,
                                color);

                            // Finished assigning a beam to user, bubble up from the nested loops
                            break;
                        }
                    }
                }
            }
        }

        private static double Magnitude(Coordinate coordinate)
        {
            var (x, y, z) = coordinate;
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        private static Coordinate Normalize(Coordinate coordinate)
        {
            var magnitude = Magnitude(coordinate);

            return new Coordinate(coordinate.X / magnitude, coordinate.Y / magnitude, coordinate.Z / magnitude);
        }

        public void Run(string filename)
        {
            var subroutine = Parser(filename);

            // Result is now an iterator
            var result = Analyzer(subroutine);

            // Print the instructions as they come
            foreach (var orderInstruction in result)
            {
                Console.WriteLine(orderInstruction);
            }
        }

        /// <summary>
        /// Utility function that parses input given the CFG.
        /// </summary>
        /// <param name="filename">file path.</param>
        /// <returns><see cref="Subroutine"/> AST node.</returns>
        private static Subroutine Parser(string filename)
        {
            var str = CharStreams.fromPath(filename);

            var lexer = new BeamLexer(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new BeamParser(tokens);

            var listenerLexer = new ConsoleErrorListener<int>();
            var listenerParser = new ConsoleErrorListener<IToken>();

            lexer.AddErrorListener(listenerLexer);
            parser.AddErrorListener(listenerParser);

            var tree = parser.subroutine();
            var visitor = new BeamVisitor();

            var subroutine = (Subroutine)visitor.Visit(tree);

            return subroutine;
        }
        
        /// <summary>
        /// Calculate angle between 3 points in 3D space.
        /// Note: assumes we want 1 vector to run from coord1 -> coord2, and the other from coord3 -> coord2.
        /// Source: https://en.wikipedia.org/wiki/Multiplication_of_vectors
        /// </summary>
        /// <param name="a">coordinate a.</param>
        /// <param name="b">coordinate b.</param>
        /// <param name="c">coordinate c.</param>
        /// <returns>Inner angle between 3 coordinates.</returns>
        private static double CalculateAlpha(Coordinate a, Coordinate b, Coordinate c)
        {
            var ba = Normalize(new Coordinate(b.X - a.X, b.Y - a.Y, b.Z - a.Z));
            var ca = Normalize(new Coordinate(c.X - a.X, c.Y - a.Y, c.Z - a.Z));

            // Dot product
            var dotProduct = ba.X * ca.X + ba.Y * ca.Y + ba.Z * ca.Z;

            // Extract the angle from the dot products
            // Converts radian to angle
            var angle = Math.Acos(dotProduct) * 180.0 / Math.PI;

            return angle;
        }
    }
}