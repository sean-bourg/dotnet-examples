namespace Gray.DistributedWriter.DocumentManagement.Devices
{
    /// <summary>
    /// Extended interface for the generic device interface. 
    /// This will adds properties and methods needed for testing 
    /// programs but not part of the original requirements.
    /// </summary>
    public partial interface IDevice
    {
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
        /// Clear folder contents.
        /// </summary>
        /// <returns></returns>
        int Clear();

    }
}
