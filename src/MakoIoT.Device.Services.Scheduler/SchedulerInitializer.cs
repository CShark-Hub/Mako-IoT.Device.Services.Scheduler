using System;
using System.Collections;
using MakoIoT.Device.Services.Interface;
using MakoIoT.Device.Services.Scheduler.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MakoIoT.Device.Services.Scheduler
{
    public class SchedulerInitializer
    {
        private readonly IScheduler _scheduler;
        private readonly IConfigurationService _config;
        private readonly ILog _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Hashtable _tasks = new();

        public SchedulerInitializer(IScheduler scheduler, IConfigurationService config, ILog logger, SchedulerOptions options, IServiceProvider serviceProvider)
        {
            _scheduler = scheduler;
            _config = config;
            _logger = logger;
            _serviceProvider = serviceProvider;

            if (options != null && options.Tasks != null)
            {
                RegisterTasks(options);
            }
        }

        private void RegisterTasks(SchedulerOptions options)
        {
            foreach (var p in options.Tasks.Keys)
            {
                _logger.Trace($"Adding task {p}");
                var task = (ITask)ActivatorUtilities.CreateInstance(_serviceProvider, (Type)options.Tasks[p]);
                _tasks.Add(p, task);
            }
        }

        public void InitializeTasks()
        {
            var config = (SchedulerConfig)_config.GetConfigSection(SchedulerConfig.SectionName, typeof(SchedulerConfig));

            if (config != null && config.Tasks != null)
            {
                foreach (var taskConfig in config.Tasks)
                {
                    try
                    {
                        _logger.Trace(
                            $"TaskId found in config: {taskConfig.TaskId}, {taskConfig.IntervalMs}");
                        if (_tasks.Contains(taskConfig.TaskId))
                        {
                            var task = (ITask)_tasks[taskConfig.TaskId];
                            _scheduler.Start(() => task.Execute(), taskConfig.IntervalMs, task.Id);
                        }
                        else
                        {
                            _logger.Warning($"Task implementation {taskConfig.TaskId} not found");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"Error initializing task {taskConfig?.TaskId}", e);
                    }
                }
            }
        }
    }
}
