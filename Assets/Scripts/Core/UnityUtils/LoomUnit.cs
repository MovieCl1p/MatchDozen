using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Core.UnityUtils
{
    public class LoomUnit
    {
        private volatile bool _isRunning = false;
        private Thread _thread = null;

        public readonly Queue<Action> Actions = new Queue<Action>();
        private readonly object _syncRoot = new object();

        private AutoResetEvent _waiter = new AutoResetEvent(false);

        public int Id { get; private set; }

        public LoomUnit(int id)
        {
            Id = id;
        }

        public void Run()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _thread = new Thread(RunActionDefault);
                _thread.Start();
            }
        }

        public bool PutAction(Action action)
        {
            lock (_syncRoot)
            {
                Actions.Enqueue(action);
                _waiter.Set();
            }

            return true;
        }

        private void RunActionDefault(object empty)
        {
            var attempts = 0;
            var maxAttempts = 20;

            while (_isRunning)
            {
                // get next action
                Action action = null;
                lock (_syncRoot)
                {
                    if (Actions.Count > 0)
                    {
                        action = Actions.Dequeue();
                    }
                    else if (attempts < maxAttempts)
                    {
                        attempts++;
                        continue;
                    }
                }

                if (action == null)
                {
                    attempts = 0;
                    _waiter.WaitOne();
                    continue;
                }

                PerformAction(action);
            }

            while (true)
            {
                Action action = null;
                lock (_syncRoot)
                {
                    if (Actions.Count > 0)
                    {
                        action = Actions.Dequeue();
                    }
                }

                if (action == null)
                {
                    break;
                }

                PerformAction(action);
            }
        }

        private void PerformAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.LogFormat("Loom exception: {0} ::: {1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
