using Gray.DistributedWriter.DocumentManagement.Devices;

namespace Gray.DistributedWriter.DocumentManagement.Schedulers
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
        /// <returns>The Device that received the file.</returns>
        IDevice Write(string name, byte[] data);
    }
}
