using Gray.DistributedWriter.DocumentManagement.Devices;
using System;

namespace Gray.DistributedWriter.DocumentManagement.Schedulers
{
    /// <summary>
    /// Create instance of a class that implements the IWriteScheduler interface.
    /// </summary>
    static public class WriteScheduler
    {
        /// <summary>
        /// Get instance of the write tester. 
        /// </summary>
        /// <param name="schedulerType"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        static public IWriteScheduler Create(SchedulerType schedulerType, IDevice[] devices)
        {
            // Create instance of scheduler:
            if (SchedulerType.Priority == schedulerType)
            {
                return new Priority(devices);
            }
            else if (SchedulerType.RoundRobin == schedulerType)
            {
                return new RoundRobin(devices);
            }

            throw new ArgumentException(String.Format("Invalid scheduler type \"{0}\"", schedulerType));
        }
    }
}
