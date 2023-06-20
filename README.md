#  Mako-IoT.Device.Services.Scheduler
Launches given tasks at configured intervals.

## Usage
1. Implement your task
```c#
public class MyServiceTask : ITask
{
    private readonly IMyService _myService;
    public string Id { get; }

    public MyServiceTask(IMyService myService)
    {
        Id = nameof(MyServiceTask); //give task a name
        _myService = myService; //inject service to call
    }
    
    public void Execute()
    {
        _myService.DoSomething(); //call the service
    }
}
```
2. Configure Scheduler
```c#
DeviceBuilder.Create()
    .AddLogging(new LoggerConfig(LogLevel.Information))
    .AddConfiguration(cfg =>
    {
        cfg.WriteDefault(SchedulerConfig.SectionName, new SchedulerConfig
        {
            //run the task every 30 seconds
            Tasks = new[]{ new SchedulerTaskConfig { TaskId = nameof(MyServiceTask), IntervalMs = 30000 }}
        });
    .AddScheduler(options =>
    {
        options.AddTask(typeof(MyServiceTask), nameof(MyServiceTask));
    })
    .Build()
    .Start();
```
See [DeviceBuilder readme](https://github.com/CShark-Hub/Mako-IoT.Device)
