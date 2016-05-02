using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Delivered.Concurrency.Throttlers
{
    internal class MultipleGroupThrottler<TSubject>
    {
        private readonly List<GroupThrottler<TSubject>> _concurrencyLimiters =
            new List<GroupThrottler<TSubject>>();

        public void AddConcurrencyLimiter(Func<TSubject, object> groupingFunc, int number)
        {
            _concurrencyLimiters.Add(new GroupThrottler<TSubject>(groupingFunc, number));
        }

        public async Task Do(Func<Task> asyncFunc, TSubject subject)
        {
            //Track which semaphores have been entered in case of exception
            var semaphoresEntered = new List<SemaphoreSlim>();

            try
            {
                //We must obtain locks in order to prevent cycles from forming
                foreach (var concurrencyLimiter in _concurrencyLimiters)
                {
                    var semaphore = concurrencyLimiter.GetSemaphoreForGroup(subject);
                    if (semaphore == null) continue;

                    semaphoresEntered.Add(semaphore);
                    await semaphore.WaitAsync().ConfigureAwait(false);
                }

                await asyncFunc().ConfigureAwait(false);
            }
            finally
            {
                foreach (var semaphore in semaphoresEntered)
                {
                    semaphore.Release();
                }
            }
        }
    }
}
