using System;

namespace Gray.DistributedWriter.DocumentManagement.Devices
{
    public readonly struct DeviceInfo
    {
        /// <summary>
        /// Get device Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Returns the current number of pending writes for this device. This number
        /// should be incremented when a write is started and decremented when a
        /// write is done.
        /// </summary>
        int PendingWrites { get; }

        /// <summary>
        /// Returns the total number of writes for this device. This number should be
        /// incremented for every write.
        /// </summary>
        int TotalWrites { get; }

        /// <summary>
        /// Returns the total number of bytes written to this device. This number should be
        /// incremented for every write.
        /// </summary>
        int TotalBytesWritten { get; }

        /// <summary>
        /// Create instance of a device info class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pendingWrites"></param>
        /// <param name="totalWrites"></param>
        /// <param name="totalBytesWritten"></param>
        public DeviceInfo(string id, int pendingWrites, int totalWrites, int totalBytesWritten)
        {
            Id = id;
            PendingWrites = pendingWrites;
            TotalWrites = totalWrites;
            TotalBytesWritten = totalWrites;
        }

        /// <summary>
        /// Convert this folder to a string reprentation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => String.Format("{0}: pending={1}, ratio={2}, byte count={3}, write count={4}", Id, PendingWrites, (0 == TotalWrites) ? 0 : TotalBytesWritten / TotalWrites, TotalBytesWritten, TotalWrites);

    }
}
