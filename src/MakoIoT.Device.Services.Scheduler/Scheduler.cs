using System;
using System.Collections;
using System.Threading;
using MakoIoT.Device.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MakoIoT.Device.Services.Scheduler
{
    public class Scheduler : IScheduler
    {
        private readonly Hashtable _timers = new Hashtable();
        private readonly ILogger _logger;

        public Scheduler(ILogger logger)
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
            string id = String.Empty;
            try
            {
                var s = (State)state;
                id = s.Id;
                _logger.LogDebug($"Invoke on {id}");
                s.Action?.Invoke();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception from {id}");
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
