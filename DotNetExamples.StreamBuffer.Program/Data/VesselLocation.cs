using System;

namespace DotNetExamples.StreamBuffer.Program.Data
{
    public readonly struct VesselLocation : IEquatable<VesselLocation>
    {
        /// <summary>
        /// Vessel id.
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// The timestamp of this location instance.
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// Vessel coordinates.
        /// </summary>
        public readonly Coordinates Coordinates;

        /// <summary>
        /// Construct instance of the VesselLocation structure.
        /// </summary>
        /// <param name="id">The vessel's id</param>
        /// <param name="timestamp">The timestamp of this coordinate</param>
        /// <param name="coordinates">The coordinates of the vessel.</param>
        public VesselLocation(string id, DateTime timestamp, Coordinates coordinates)
        {
            Id = id;
            Timestamp = timestamp;
            Coordinates = coordinates;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(VesselLocation obj) => (Id == obj.Id) && (Timestamp == obj.Timestamp) && Coordinates.Equals(obj.Coordinates);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj) => (obj is VesselLocation) ? this.Equals((VesselLocation)obj) : false;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool operator ==(VesselLocation t1, VesselLocation t2) => t1.Equals(t2);

        /// <summary>
        /// Determines whether the specified object is not equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool operator !=(VesselLocation t1, VesselLocation t2) => !t1.Equals(t2);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Convert this vessel location instance into a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => String.Format("{0} @ {1} = {2}", Id, Timestamp.ToString(), Coordinates);
    }
}
