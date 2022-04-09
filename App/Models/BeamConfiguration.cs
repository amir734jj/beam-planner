using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class BeamConfiguration
    {
        /// <summary>
        /// Max number of mountainous beams of a satellite
        /// </summary>
        public int MaxSimultaneousBeams { get; set; }

        /// <summary>
        /// User angle pointing to the satellite
        /// </summary>
        public double UserAngle { get; set; }

        /// <summary>
        /// Max interference with non-spacex satellites
        /// </summary>
        public int MaxInterferesInterference { get; set; }

        /// <summary>
        /// Max self interference of spacex satellite
        /// </summary>
        public int MaxSelfInterference { get; set; }

        /// <summary>
        /// Number of colors available to each satellite
        /// </summary>
        [Range(0, 26)]
        public int NumColors { get; set; }
    }
}