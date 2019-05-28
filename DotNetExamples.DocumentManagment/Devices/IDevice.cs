namespace DotNetExamples.DocumentManagment.Devices
{
    /// <summary>
    /// Generic device interface.
    /// </summary>
    public partial interface IDevice
    {
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
        /// Get device Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// True if access to the ICollection is synchronized (thread safe); otherwise, false.
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the IDevice.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Get this devices current state.
        /// </summary>
        /// <returns></returns>
        DeviceInfo GetInfo();

        /// <summary>
        /// Writes a new file to the device
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file's data</param>
        void Write(string name, byte[] data);
    }
}
