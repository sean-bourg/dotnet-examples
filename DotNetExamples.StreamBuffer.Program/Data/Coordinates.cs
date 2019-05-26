using System;

namespace DotNetExamples.StreamBuffer.Program.Data
{
    /// <summary>
    /// GPS Coordinates.
    /// </summary>
    public readonly struct Coordinates : IEquatable<Coordinates>
    {
        /// <summary>
        /// The longitude portion of this coordinate instance.
        /// </summary>
        public readonly double Longitude;

        /// <summary>
        /// The latitude portion of this coordinate instance.
        /// </summary>
        public readonly double Latitude;

        /// <summary>
        /// Create instance of the Coordinate.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(Coordinates obj) => (Longitude == obj.Longitude) && (Latitude == obj.Latitude);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj) => (obj is Coordinates) ? this.Equals((Coordinates)obj) : false;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool operator ==(Coordinates t1, Coordinates t2) => t1.Equals(t2);

        /// <summary>
        /// Determines whether the specified object is not equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool operator !=(Coordinates t1, Coordinates t2) => !t1.Equals(t2);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => base.GetHashCode();
        
        /// <summary>
        /// Convert coordinates to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => String.Format("({0},{1})", Latitude, Longitude);
    }
}
