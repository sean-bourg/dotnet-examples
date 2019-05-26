using DotNetExamples.StreamBuffer.Program.Data;
using System;

namespace DotNetExamples.StreamBuffer.Program.Generators
{
    /// <summary>
    /// Generate random vessel location.
    /// </summary>
    public class VesselLocationGenerator : IGenerator<VesselLocation>
    {
        /// <summary>
        /// Name of this generator
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Random generator for generation changes in coordinates.
        /// </summary>
        protected readonly Random Random;

        /// <summary>
        /// The last oordiantes of the vessel
        /// </summary>
        protected Coordinates Coordinates;

        /// <summary>
        /// Create instance of a named generator.
        /// </summary>
        /// <param name="name"></param>
        public VesselLocationGenerator(string name)
        {
            Name = name;
            Random = new Random();
            Coordinates = new Coordinates(
                Random.Next(-90000, 90000) / 1000d,
                -72d - Random.Next(-180000, 180000) / 1000d
            );
        }

        /// <summary>
        /// Get next vessel coordinate instance.
        /// </summary>
        /// <returns></returns>
        public VesselLocation Next()
        {
            double latitudeOffset = (Random.Next(-100, 100) / 1000d) * Random.Next(-1, 0);
            double latitude = Coordinates.Latitude + latitudeOffset;
            if ((-90 > latitude) || (90 < latitude))
            {
                latitudeOffset *= -1;
                latitude = Coordinates.Latitude + latitudeOffset;
            }

            double longitudeOffset = (Random.Next(-100, 100) / 1000d) * Random.Next(-1, 0);
            double longitude = Coordinates.Longitude + longitudeOffset;
            if ((-180 > longitude) || (180 < longitude))
            {
                longitudeOffset *= -1;
                longitude = Coordinates.Longitude + longitudeOffset;
            }

            Coordinates = new Coordinates(latitude, longitude);
            return new VesselLocation(Name, DateTime.Now, Coordinates);
        }
    }
}
