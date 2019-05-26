using System;
using System.IO;
using System.Threading.Tasks;

namespace Gray.DistributedWriter.DocumentManagement.Devices
{
    /// <summary>
    /// Standard folder device.
    /// </summary>
    public class FolderDevice : IDevice
    {
        /// <summary>
        /// Returns the current number of pending writes for this device. This number
        /// should be incremented when a write is started and decremented when a
        /// write is done.
        /// </summary>
        public int PendingWrites { get; protected set; } = 0;

        /// <summary>
        /// Returns the total number of writes for this device. This number should be
        /// incremented for every write.
        /// </summary>
        public int TotalWrites { get; protected set; } = 0;

        /// <summary>
        /// Returns the total number of bytes written to this device. This number should be
        /// incremented for every write.
        /// </summary>
        public int TotalBytesWritten { get; protected set; } = 0;

        /// <summary>
        /// Directory to write to for this object.
        /// </summary>
        public DirectoryInfo DirectoryInfo { get; }
        
        /// <summary>
        /// Latency to use for this folder object (testing/logging).
        /// </summary>
        public Latency Latency { get; set; }

        /// <summary>
        /// Random generator for this object.
        /// </summary>
        public Random Random { get; } = new Random();
        /// <summary>
        /// Id of this device.
        /// </summary>
        public string Id { get => String.Format("fd{0:##}", DirectoryInfo.Name); }

        /// <summary>
        /// True if access to the ICollection is synchronized (thread safe); otherwise, false.
        /// </summary>
        public bool IsSynchronized { get => true; }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the IDevice.
        /// </summary>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// Create instance of the folder device.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="latency"></param>
        public FolderDevice(DirectoryInfo directoryInfo, Latency latency)
        {
            DirectoryInfo = directoryInfo;
            Latency = latency;
        }

        /// <summary>
        /// Create instance of the folder device without latency.
        /// </summary>
        /// <param name="directoryInfo"></param>
        public FolderDevice(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
            Latency = new Latency(0, 0);
        }

        /// <summary>
        /// Write file. This fuction write file in 512b chunks.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public virtual void Write(string name, byte[] data)
        {
            PendingWrites++;
            lock (SyncRoot)
            {
                int delay = Latency.Next(Random);

                // Testing output
                DateTime startTime = DateTime.Now;
                double fileSize = (data.Length / 1024f);

                // delay based on filesize (otherwise it will always be 0 seconds).
                delay = (int) Math.Floor(delay * fileSize);
                Console.WriteLine("[{0}] fi {1}(\"{2}\") size={3:0.##}kb, latency={4}ms", startTime.ToFileTime(), Id, name, fileSize, delay);

                using (BinaryWriter file = new BinaryWriter(new FileStream(Path.Combine(DirectoryInfo.FullName, name), FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    Task.Delay(delay).Wait();
                    file.Write(data);
                }
                TotalWrites++;
                TotalBytesWritten += data.Length;

                int seconds = DateTime.Now.Subtract(startTime).Seconds;
                Console.WriteLine("[{0}] fr {1}(\"{2}\") throughput rate={3:#0.###0}", DateTime.Now.ToFileTime(), Id, name, (0 == seconds) ? 0 : (fileSize / seconds));
            }
            PendingWrites--;
        }
                
        /// <summary>
        /// Get this device's info.
        /// </summary>
        /// <returns></returns>
        public DeviceInfo GetInfo()
        {
            return new DeviceInfo(Id, PendingWrites, TotalWrites, TotalBytesWritten);
        }

        /// <summary>
        /// Get string representation of this device.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Id;

        /// <summary>
        /// Clear folder contents.
        /// </summary>
        /// <returns></returns>
        public int Clear()
        {
            int count = 0;
            foreach (FileInfo file in DirectoryInfo.GetFiles())
            {
                file.Delete();
                count++;
            }
            return count;
        }
    }
}
