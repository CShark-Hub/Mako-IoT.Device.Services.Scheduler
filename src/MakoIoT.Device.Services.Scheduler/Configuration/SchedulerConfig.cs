namespace MakoIoT.Device.Services.Scheduler.Configuration
{
    public class SchedulerConfig
    {
        public SchedulerTaskConfig[] Tasks { get; set; }
        public static string SectionName => "Scheduler";
    }

    public class SchedulerTaskConfig
    {
        public string TaskId { get; set; }
        public int IntervalMs { get; set; }
    }
}
