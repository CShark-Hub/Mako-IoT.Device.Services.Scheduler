using MakoIoT.Device.Services.Interface;
using nanoFramework.DependencyInjection;

namespace MakoIoT.Device.Services.Scheduler.Extensions
{
    public delegate void SchedulerConfigurator(SchedulerOptions options);

    public static class DeviceBuilderExtension
    {
        public static IDeviceBuilder AddScheduler(this IDeviceBuilder builder, SchedulerConfigurator configurator)
        {
            builder.Services.AddSingleton(typeof(IScheduler), typeof(Scheduler));

            var options = new SchedulerOptions();
            configurator(options);
            builder.Services.AddSingleton(typeof(SchedulerOptions), options);

            builder.DeviceStarting += Builder_DeviceStarting;

            return builder;
        }

        private static void Builder_DeviceStarting(IDevice sender)
        {
            var initializer = (SchedulerInitializer)ActivatorUtilities.CreateInstance(sender.ServiceProvider, typeof(SchedulerInitializer));
            initializer.InitializeTasks();
        }
    }
}
