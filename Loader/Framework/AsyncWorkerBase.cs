using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using Serilog;

namespace NPPES.Loader.Framework
{
    public abstract class AsyncWorkerBase<T>
    {
        protected readonly ConcurrentQueue<T> jobs;

        private AutoResetEvent handle;
        private Thread t;
        private static int global_index=0;
        private readonly int index=0;
        public AsyncWorkerBase()
        {
            index = global_index++;
            t = new Thread(new ThreadStart(Run));
            t.Name = $"{typeof(T).Name}-Worker-{index}";
            t.IsBackground = true;
            t.Priority = ThreadPriority.Highest;
            jobs = new ConcurrentQueue<T>();
            handle = new AutoResetEvent(false);
        }

        public virtual void Start()
        {
            t.Start();
        }

        public void Submit(T info)
        {
            jobs.Enqueue(info);
            handle.Set();
        }

        public virtual void Run()
        {
            while (true)
            {
                if (jobs.IsEmpty)
                    handle.WaitOne();
                else
                {
                    var success = jobs.TryDequeue(out T info);
                    if (success)
                    {
                        Log.Information($"[{t.Name}] - Started processing {info}");
                        Process(info);
                        Log.Information($"[{t.Name}] - Completed processing {info}");
                    }
                }
            }
        }

        protected abstract void Process(T info);

        public virtual string Pending()
        {
            return this.jobs.Count().ToString();
        }

    }
}
