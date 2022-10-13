using System;
using System.Collections;
using MakoIoT.Device.Services.DependencyInjection;
using MakoIoT.Device.Services.Interface;
using MakoIoT.Device.Services.Scheduler.Configuration;
using Microsoft.Extensions.Logging;

namespace MakoIoT.Device.Services.Scheduler
{
    public class SchedulerInitializer
    {
        private readonly IScheduler _scheduler;
        private readonly IConfigurationService _config;
        private readonly ILogger _logger;

        private readonly Hashtable _tasks = new();

        public SchedulerInitializer(IScheduler scheduler, IConfigurationService config, ILogger logger, SchedulerOptions options)
        {
            _scheduler = scheduler;
            _config = config;
            _logger = logger;

            RegisterTasks(options);
        }

        private void RegisterTasks(SchedulerOptions options)
        {
            foreach (var p in options.Tasks.Keys)
            {
                _logger.LogDebug($"Adding task {p}");
                var task = (ITask)DI.BuildUp((Type)options.Tasks[p]);
                _tasks.Add(p, task);
            }
        }

        public void InitializeTasks()
        {
            var config = (SchedulerConfig)_config.GetConfigSection(SchedulerConfig.SectionName, typeof(SchedulerConfig));

            foreach (var taskConfig in config.Tasks)
            {
                try
                {
                    _logger.LogDebug(
                        $"TaskId found in config: {taskConfig.TaskId}, {taskConfig.IntervalMs}");
                    if (_tasks.Contains(taskConfig.TaskId))
                    {
                        var task = (ITask)_tasks[taskConfig.TaskId];
                        _scheduler.Start(() => task.Execute(), taskConfig.IntervalMs, task.Id);
                    }
                    else
                    {
                        _logger.LogWarning($"Task implementation {taskConfig.TaskId} not found");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error initializing task {taskConfig?.TaskId}");
                }
            }
        }
    }
}
