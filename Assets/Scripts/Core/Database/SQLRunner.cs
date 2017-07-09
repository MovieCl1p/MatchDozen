using Core.UnityUtils;
using SQLite4Unity3d;
using System;

namespace Core.Database
{
    public class SQLRunner
    {
        private const int DATABASE_THREAD = 2;

        private static SQLRunner _instance;

        protected object LockObject = new object();

        public SQLiteConnection Connection { get; private set; }

        public static SQLRunner Instance
        {
            get
            {
                return _instance ?? (_instance = new SQLRunner());
            }
        }

        public void AsyncCall(Action action, Action onSuccess = null, Action onError = null)
        {
            Loom.RunAsyncToDefaultThread(DATABASE_THREAD, () =>
            {
                lock (LockObject)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception)
                    {
                        if (onError != null)
                        {
                            onError();
                        }

                        throw;
                    }
                }

                Loom.QueueOnMainThread(() =>
                {
                    if (onSuccess != null)
                    {
                        onSuccess();
                    }
                });
            });
        }
    }
}
