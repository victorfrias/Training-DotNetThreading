using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Threading.HandsOn.Entities;

namespace Threading.HandsOn
{
    public abstract class Worker : IWorker
    {
        private static readonly object _locker = new object();

        public TResult AtomicTask<TResult>(CancellationToken ct, Func<TResult> func)
        {
            try
            {                
                Thread.CurrentThread.Priority = ThreadPriority.Highest;

                while (!ct.IsCancellationRequested)
                {                   
                    lock (_locker)
                    {
                        return func.Invoke();                
                    }                                         
                }                
            }
            catch { }

            return func.Invoke();
        }
    }
}
