using Gray.DistributedWriter.DocumentManagement.Devices;
using System;
using System.Linq;

namespace Gray.DistributedWriter.DocumentManagement.Schedulers
{
    /// <summary>
    /// Write schedule based on a combination of device pending writes and historical throughput.
    /// </summary>
    public class Priority : IWriteScheduler
    {
        /// <summary>
        /// Gets an object that can be used to synchronize access to the IWriteScheduler.
        /// </summary>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// True if access to the IWriteScheduler is synchronized (thread safe)  otherwise, false.
        /// </summary>
        public bool IsSynchronized { get => true; }
        /// <summary>
        /// Protected instance of the devices list.
        /// </summary>
        private IDevice[] _devices;

        /// <summary>
        /// List of registered devices in order by write priority.
        /// </summary>
        public IDevice[] Devices
        {

            get => (
                from d in _devices
                orderby d.PendingWrites ascending, CalculateWritePriority(d.TotalWrites, d.TotalBytesWritten) ascending
                select d
            ).ToArray<IDevice>();
            protected set => _devices = value;
        }

        /// <summary>
        /// Construct instance of the priority scheduler.
        /// </summary>
        /// <param name="devices"></param>
        public Priority(IDevice[] devices)
        {
            if (0 == devices.Length)
            {
                throw new ArgumentException("No devices registered");
            }
            Devices = devices;
        }

        /// <summary>
        /// Writes a new file to the underlying set of devices.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file's data</param>
        /// <returns>The Device that received the file.</returns>
        public IDevice Write(string name, byte[] data)
        {
            lock (SyncRoot)
            {
                IDevice device = Devices.FirstOrDefault();
                device.Write(name, data);
                return device;
            }
        }

        /// <summary>
        /// Calculate device priority using the number of writes and the byte count.
        /// Aa an improvement the device could be changed to keep a buffer of pending writes that is used to 
        /// calculate estimaed write times base on the device speed and bytes pending write (a faster device with multiple 
        /// small files may execute quicker than a slower device with less pending writes).
        /// This design also does not take into consideration changing write writes over time or fault tolerance due to 
        /// network disconnects - changes to the underlying interfaces would be needed for this to keep track of a "recent write history" 
        /// to calculate the difference between historical write rates and current ones.
        /// </summary>
        /// <param name="writeCount"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        protected double CalculateWritePriority(int writeCount, int byteCount) => (writeCount == 0) ? 0 : byteCount / writeCount;

        /// <summary>
        /// Return disk array - used to print disks to user.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DeviceInfo[] ToArray()
        {
            lock (SyncRoot)
            {
                return Devices.Select(device => device.GetInfo())
                    .ToArray();
            }
        }

    }
}
