using System;
using System.IO;
using System.Threading.Tasks;

namespace DotNetExamples.DocumentManagment.Devices
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
            int index = 0;
            int offset = (512 > data.Length) ? data.Length : 512;

            // Testing output
            int delay = Latency.Next(Random);
            using (BinaryWriter file = new BinaryWriter(new FileStream(Path.Combine(DirectoryInfo.FullName, name), FileMode.OpenOrCreate, FileAccess.Write)))
            {
                while (index < offset)
                {
                    Task.Delay(delay).Wait();
                    file.Write(data, index, (offset - index));
                    int bufferSize = offset - index;

                    // Update buffer write region
                    index = offset;
                    offset += 512;
                    offset = (offset > data.Length) ? data.Length : offset;
                }
            }
            TotalWrites++;
            TotalBytesWritten += data.Length;
            PendingWrites--;
        }
    }
}
