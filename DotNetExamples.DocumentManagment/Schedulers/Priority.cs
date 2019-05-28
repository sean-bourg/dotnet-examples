using DotNetExamples.DocumentManagment.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetExamples.DocumentManagment.Schedulers
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
        List<IDevice> _devices;

        /// <summary>
        /// List of registered devices in order by write priority.
        /// </summary>
        List<IDevice> Devices
        {
            get => _devices
                .Select(x => x)
                .OrderBy(d => d.PendingWrites)
                .OrderBy(d => CalculateWritePriority(d.TotalWrites, d.TotalBytesWritten))
                .ToList<IDevice>();

            set => _devices = value;
        }

        /// <summary>
        /// Construct instance of the priority scheduler.
        /// </summary>
        /// <param name="devices"></param>
        public Priority()
        {
            Devices = new List<IDevice>();
        }

        /// <summary>
        /// Register an output device.
        /// </summary>
        /// <param name="device"></param>
        public void Register(IDevice device) => Devices.Add(device);


        /// <summary>
        /// Writes a new file to the underlying set of devices.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file's data</param>
        /// <returns>The device id that received the file.</returns>
        public string Write(string name, byte[] data)
        {
            IDevice device = Devices.FirstOrDefault();
            device.Write(name, data);
            return device.Id;
        }

        /// <summary>
        /// Return disk array - used to print disks to user.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DeviceInfo[] ToArray() => Devices.Select(device => device.GetInfo()).ToArray();

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
        double CalculateWritePriority(int writeCount, int byteCount) => (writeCount == 0) ? 0 : byteCount / writeCount;
    }
}
