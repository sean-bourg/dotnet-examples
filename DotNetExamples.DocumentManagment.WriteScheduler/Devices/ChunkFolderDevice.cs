using System;
using System.IO;
using System.Threading.Tasks;

namespace Gray.DistributedWriter.DocumentManagement.Devices
{
    /// <summary>
    /// Folder device that writes using 512b chunks. By apply latency between writes 
    /// this will providate drastically different throughput than the base folder device.
    /// </summary>
    public class ChunkFolderDevice : FolderDevice, IDevice
    {
        /// <summary>
        /// Create instance of the folder device.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="latency"></param>
        public ChunkFolderDevice(DirectoryInfo directoryInfo, Latency latency) : base(directoryInfo, latency) { }

        /// <summary>
        /// Create instance of the folder device without latency.
        /// </summary>
        /// <param name="directoryInfo"></param>
        public ChunkFolderDevice(DirectoryInfo directoryInfo) : base(directoryInfo) { }

        /// <summary>
        /// Write file. This fuction write file in 512b chunks.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public override void Write(string name, byte[] data)
        {
            PendingWrites++;
            lock (SyncRoot)
            {
                int index = 0;
                int offset = (512 > data.Length) ? data.Length : 512;

                // Testing output
                int delay = Latency.Next(Random);

                DateTime startTime = DateTime.Now;
                double fileSize = (data.Length / 1024f);
                int blockCount = 0;
                Console.WriteLine("[{0}] fi {1}(\"{2}\") size={3:#.#0}kb, latency={4}ms", startTime.ToFileTime(), Id, name, fileSize, delay);

                using (BinaryWriter file = new BinaryWriter(new FileStream(Path.Combine(DirectoryInfo.FullName, name), FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    while (index < offset)
                    {
                        Task.Delay(delay).Wait();
                        file.Write(data, index, (offset - index));

                        int bufferSize = offset - index;
                        Console.WriteLine("[{0}] fw {1}(\"{2}\") {3,3}b=[{4},{5}]", DateTime.Now.ToFileTime(), Id, name, bufferSize, index, offset);
                        blockCount++;

                        // Update buffer write region
                        index = offset;
                        offset += 512;
                        offset = (offset > data.Length) ? data.Length : offset;
                    }
                }
                TotalWrites++;
                TotalBytesWritten += data.Length;

                int seconds = DateTime.Now.Subtract(startTime).Seconds;
                Console.WriteLine("[{0}] fr {1}(\"{2}\") blocks={3}, throughput rate={4:#0.###0}", DateTime.Now.ToFileTime(), Id, name, blockCount, (0 == seconds) ? 0 : (fileSize / seconds));
            }
            PendingWrites--;
        }
    }
}
