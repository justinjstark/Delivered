using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Delivered.Tests.Fakes
{
    public interface IControllable
    {
        Queue<Process> Processes { get; }
        AutoResetEvent ProcessStarted { get; }
    }

    public class Process
    {
        public AsyncAutoResetEvent Continue;
        public Task Task;
    }

    public class Process<TResult> : Process
    {
        public new Task<TResult> Task;
    }

    public class Controller<T> where T : IControllable
    {
        private T Object { get; }

        public Controller(T obj)
        {
            Object = obj;
        }

        public bool HappenAsynchronously(Func<Task> func1, Func<Task> func2)
        {
            //Call func1 and wait for it to initialize
            WaitForStart(func1);

            //Run func2 to completion
            var process2 = WaitForStart(func2);
            process2.Continue.Set();
            var success = process2.Task.Wait(new TimeSpan(0, 0, 0, 0, 200));

            return success;
        }

        public bool HappenSynchronously(Func<Task> func1, Func<Task> func2)
        {
            WaitForStart(func1);

            var success = WaitForStart(func2, new TimeSpan(0, 0, 0, 0, 200));

            return !success;
        }

        public Process WaitForStart(Func<Task> func)
        {
            func();

            Object.ProcessStarted.WaitOne();

            return Object.Processes.Dequeue();
        }

        public bool WaitForStart(Func<Task> func, TimeSpan timeout)
        {
            return Object.ProcessStarted.WaitOne(timeout);
        }
    }

    public class FakeLoggedDeliverer<TDistributable, TEndpoint> : Deliverer<TDistributable, TEndpoint>, IControllable
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        public Queue<Process> Processes { get; } = new Queue<Process>();
        
        public AutoResetEvent ProcessStarted { get; } = new AutoResetEvent(false);
        
        public FakeLoggedDeliverer()
        {
        }

        public FakeLoggedDeliverer(IDictionary<Func<TEndpoint, object>, int> groupingFuncs)
        {
            foreach (var keyValuePair in groupingFuncs)
            {
                MaximumConcurrentDeliveries(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint)
        {
            var process = new Process
            {
                Continue = new AsyncAutoResetEvent()
            };

            Processes.Enqueue(process);

            ProcessStarted.Set();

            var task = process.Continue.WaitAsync();
            process.Task = task;
            await task;
        }
    }
}
