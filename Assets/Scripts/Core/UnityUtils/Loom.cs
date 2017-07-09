using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UnityUtils
{
    public class Loom : MonoBehaviour
    {
        private static int _maxThreads = 2;

        private static Loom _current;
        private static bool _initialized;

        private static readonly Dictionary<int, LoomUnit> ActiveUnits = new Dictionary<int, LoomUnit>();
        private readonly List<Action> _actions = new List<Action>();

        public static Loom Current
        {
            get
            {
                Initialize();
                return _current;
            }
        }

        public static void RunAsyncToDefaultThread(int key, Action action)
        {
            Initialize();

            // add work
            LoomUnit loomUnit;

            if (!ActiveUnits.TryGetValue(key, out loomUnit))
            {
                loomUnit = new LoomUnit(key);
                ActiveUnits[key] = loomUnit;

                loomUnit.Run();
            }

            loomUnit.PutAction(action);
        }

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time)
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }

        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                return;
            }

            _initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }
    }
}
