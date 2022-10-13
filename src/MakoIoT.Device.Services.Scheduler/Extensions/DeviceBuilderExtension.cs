using MakoIoT.Device.Services.DependencyInjection;
using MakoIoT.Device.Services.Interface;

namespace MakoIoT.Device.Services.Scheduler.Extensions
{
    public delegate void SchedulerConfigurator(SchedulerOptions options);

    public static class DeviceBuilderExtension
    {
        public static IDeviceBuilder AddScheduler(this IDeviceBuilder builder, SchedulerConfigurator configurator)
        {
            DI.RegisterSingleton(typeof(IScheduler), typeof(Scheduler));

            var options = new SchedulerOptions();
            configurator(options);
            DI.RegisterInstance(typeof(SchedulerOptions), options);

            builder.DeviceStarting += Builder_DeviceStarting;

            return builder;
        }

        private static void Builder_DeviceStarting(object sender, System.EventArgs e)
        {
            var initializer = (SchedulerInitializer)DI.BuildUp(typeof(SchedulerInitializer));
            initializer.InitializeTasks();
        }
    }
}
