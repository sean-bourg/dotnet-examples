using DotNetExamples.DocumentManagment.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetExamples.DocumentManagment.Schedulers
{
    /// <summary>
    /// Round robin write scheduler.
    /// </summary>
    public class RoundRobin : IWriteScheduler
    {
        /// <summary>
        /// List of registered devices.
        /// </summary>
        List<IDevice> Devices { get; set; }

        /// <summary>
        /// Current index for active device.
        /// </summary>
        int CurrentIndex { get; set; } = 0;

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
        public RoundRobin()
        {
            Devices = new List<IDevice>();
        }

        /// <summary>
        /// Register write device.
        /// </summary>
        /// <param name="device"></param>
        public void Register(IDevice device) => Devices.Add(device);

        /// <summary>
        /// Writes a new file to the underlying set of devices.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file's data</param>
        /// <returns>The device id of the device that received the file.</returns>
        public string Write(string name, byte[] data)
        {
            string path = Devices[CurrentIndex].Id;
            Devices[CurrentIndex].Write(name, data);
            CurrentIndex = (CurrentIndex + 1) % Devices.Count;
            return path;
        }

        /// <summary>
        /// Return disk array - used to print disks to user.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DeviceInfo[] ToArray() => Devices.Select((value, index) => new
        {
            Value = value,
            Priority = (Devices.Count() - CurrentIndex + index) % Devices.Count(),
        })
            .OrderBy(record => record.Priority)
            .Select(record => record.Value.GetInfo())
            .ToArray();
    }
}
