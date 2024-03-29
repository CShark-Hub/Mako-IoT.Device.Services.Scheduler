﻿using System;
using System.Collections;
using System.Threading;
using MakoIoT.Device.Services.Interface;

namespace MakoIoT.Device.Services.Scheduler
{
    public class Scheduler : IScheduler
    {
        private readonly Hashtable _timers = new Hashtable();
        private readonly ILog _logger;

        public Scheduler(ILog logger)
        {
            _logger = logger;
        }

        public void Start(Action action, int interval, string id)
        {
            Stop(id);

            _timers.Add(id, new Timer(TimerElapsed, new State(action, id), 0, interval));
        }
        public void Stop(string id)
        {
            if (_timers.Contains(id))
            {
                (_timers[id] as Timer)?.Dispose();
                _timers.Remove(id);
            }
        }

        public void StopAll()
        {
            foreach (var key in _timers.Keys)
            {
                Stop((string)key);
            }
        }


        private void TimerElapsed(object state)
        {
            var id = string.Empty;
            try
            {
                var s = (State)state;
                id = s.Id;
                _logger.Trace($"Invoke on {id}");
                s.Action?.Invoke();
            }
            catch (Exception e)
            {
                _logger.Error($"Exception from {id}", e);
            }
        }

        private struct State
        {
            public State(Action action, string id)
            {
                Action = action;
                Id = id;
            }

            public Action Action { get; }
            public string Id { get; }
        }
    }
}
