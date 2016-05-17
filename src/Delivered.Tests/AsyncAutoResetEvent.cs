using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivered.Tests
{
    public class AsyncAutoResetEvent
    {
        private static readonly Task Completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private bool _signaled;

        public Task WaitAsync()
        {
            lock (_waiters)
            {
                if (_signaled)
                {
                    _signaled = false;
                    return Completed;
                }

                var waiter = new TaskCompletionSource<bool>();

                _waiters.Enqueue(waiter);

                return waiter.Task;
            }
        }

        public void Set()
        {
            TaskCompletionSource<bool> waiterToRelease = null;

            lock (_waiters)
            {
                if (_waiters.Count > 0)
                {
                    waiterToRelease = _waiters.Dequeue();
                }
                else if (!_signaled)
                {
                    _signaled = true;
                }
            }

            waiterToRelease?.SetResult(true);
        }
    }

    public class AsyncAutoResetEvent<TResult>
    {
        private readonly Queue<TaskCompletionSource<TResult>> _waiters = new Queue<TaskCompletionSource<TResult>>();
        private readonly Queue<TResult> _results = new Queue<TResult>();

        public Task<TResult> WaitAsync()
        {
            lock (_waiters)
            {
                if (_results.Any())
                {
                    return Task.FromResult(_results.Dequeue());
                }

                var waiter = new TaskCompletionSource<TResult>();

                _waiters.Enqueue(waiter);

                return waiter.Task;
            }
        }

        public void Set(TResult result)
        {
            TaskCompletionSource<TResult> waiterToRelease = null;

            lock (_waiters)
            {
                if (_waiters.Any())
                {
                    waiterToRelease = _waiters.Dequeue();
                }
                else
                {
                    _results.Enqueue(result);
                }
            }

            waiterToRelease?.SetResult(result);
        }
    }

}
