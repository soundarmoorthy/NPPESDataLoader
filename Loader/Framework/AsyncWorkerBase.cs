using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace NPPES.Loader.Framework
{
    public abstract class AsyncWorkerBase<T>
    {
        protected readonly ConcurrentQueue<T> jobs;

        private AutoResetEvent handle;
        private Thread t;
        public AsyncWorkerBase()
        {
            t = new Thread(new ThreadStart(Run));
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
                        Process(info);
                }
            }
        }

        protected abstract void Process(T info);

        internal virtual string Pending()
        {
            return this.jobs.Count().ToString();
        }

    }
}
