using System.Collections.Generic;
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
}
