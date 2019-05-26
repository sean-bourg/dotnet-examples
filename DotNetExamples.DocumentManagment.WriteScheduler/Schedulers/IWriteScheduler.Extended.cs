using Gray.DistributedWriter.DocumentManagement.Devices;

namespace Gray.DistributedWriter.DocumentManagement.Schedulers
{
    /// <summary>
    /// Extended features for the Write scheduler interface.
    /// </summary>
    public partial interface IWriteScheduler
    {
        /// <summary>
        /// True if access to the IWriteScheduler is synchronized (thread safe)  otherwise, false.
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the IWriteScheduler.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Return list of devices registered to the write scheduler.
        /// </summary>
        /// <returns></returns>
        DeviceInfo[] ToArray();
    }
}
