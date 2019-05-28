using DotNetExamples.DocumentManagment.Devices;

namespace DotNetExamples.DocumentManagment.Schedulers
{
    /// <summary>
    /// Write scheduler interface. 
    /// </summary>
    public partial interface IWriteScheduler
    {
        /// <summary>
        /// Writes a new file to the underlying set of devices.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file's data</param>
        /// <returns>The device id that the file was written to.</returns>
        string Write(string name, byte[] data);

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

        /// <summary>
        /// Register output devices to write scheduler.
        /// </summary>
        /// <param name="device"></param>
        void Register(IDevice device);
    }
}
