using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class BeamConfiguration
    {
        /// <summary>
        /// Max number of mountainous beams of a satellite
        /// </summary>
        public double MaxSimultaneousBeams { get; set; } = 32;

        /// <summary>
        /// User angle pointing to the satellite
        /// </summary>
        public double UserAngle { get; set; } = 45;

        /// <summary>
        /// Max interference with non-spacex satellites
        /// </summary>
        public int MaxInterferesInterference { get; set; } = 20;

        /// <summary>
        /// Max self interference of spacex satellite
        /// </summary>
        public int MaxSelfInterference { get; set; } = 10;

        /// <summary>
        /// Number of colors available to each satellite
        /// </summary>
        [Range(0, 26)]
        public int NumColors { get; set; } = 4;
    }
}