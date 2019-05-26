using Gray.DistributedWriter.DocumentManagement.Devices;
using System;
using System.Linq;

namespace Gray.DistributedWriter.DocumentManagement.Schedulers
{
    /// <summary>
    /// Round robin write scheduler.
    /// </summary>
    public class RoundRobin : IWriteScheduler
    {
        /// <summary>
        /// List of registered devices.
        /// </summary>
        public IDevice[] Devices { get; protected set; }

        /// <summary>
        /// Current index for active device.
        /// </summary>
        public int CurrentIndex { get; protected set; } = -1;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the IWriteScheduler.
        /// </summary>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// True if access to the IWriteScheduler is synchronized (thread safe)  otherwise, false.
        /// </summary>
        public bool IsSynchronized { get => true; }

        /// <summary>
        /// Construct instance of the round robin write scheduler.
        /// </summary>
        /// <param name="devices"></param>
        public RoundRobin(IDevice[] devices)
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
                CurrentIndex = (CurrentIndex + 1) % Devices.Length;
                Devices[CurrentIndex].Write(name, data);
                return Devices[CurrentIndex];
            }
        }

        /// <summary>
        /// Return disk array - used to print disks to user.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DeviceInfo[] ToArray()
        {
            lock (SyncRoot)
            {
                return Devices.Select((value, index) => new
                {
                    Value = value,
                    Priority = (Devices.Length - (CurrentIndex + 1) + index) % Devices.Length,
                })
                .OrderBy(record => record.Priority)
                .Select(record => record.Value.GetInfo())
                .ToArray();
            }
        }
    }
}
