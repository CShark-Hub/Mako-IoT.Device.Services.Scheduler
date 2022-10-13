using System;
using System.Collections;

namespace MakoIoT.Device.Services.Scheduler
{
    public class SchedulerOptions
    {
        internal Hashtable Tasks = new();

        public void AddTask(Type taskType, string id)
        {
            Tasks.Add(id, taskType);
        }
    }
}
