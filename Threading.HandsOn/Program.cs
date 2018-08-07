using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Threading.HandsOn.Entities;

namespace Threading.HandsOn
{
    class Program
    {
        static void Main(string[] args)
        {
            var localCancelToken = new CancellationTokenSource();
            var worker1 = new DoSomethingWorker();


            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var contextList = new List<Worker>();
            contextList.Add(worker1);

            var taskList = new List<Task<bool>>();

            contextList.ForEach(_ =>
                        taskList.Add(
                            Task<bool>.Factory.StartNew(_.AtomicTask(localCancelToken.Token, () => { return true; }), localCancelToken.Token)
                            )
                        );

            var timeout = TimeSpan.FromMinutes(1);

            var i = Task.WaitAny(taskList.ToArray(), timeout);

            localCancelToken.Cancel();

            var result = (i > -1) ? taskList[i].Result : false;

            Task.WaitAll(taskList.ToArray());

            taskList.ForEach(_ => _.Dispose());

            stopWatch.Stop();
        }
    }
}
