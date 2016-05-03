using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Delivered.Concurrency.Throttlers
{
    public class GroupThrottler<TSubject>
    {
        private readonly Func<TSubject, object> _groupingFunc;

        private readonly int _concurrencyLimit;

        private readonly ConcurrentDictionary<object, SemaphoreSlim> _semaphores
            = new ConcurrentDictionary<object, SemaphoreSlim>();

        public GroupThrottler(Func<TSubject, object> groupingFunc, int concurrencyLimit)
        {
            if (concurrencyLimit <= 0)
            {
                throw new ArgumentException(@"Concurrency limit must be greater than 0.", nameof(concurrencyLimit));
            }

            _groupingFunc = groupingFunc;
            _concurrencyLimit = concurrencyLimit;
        }

        public SemaphoreSlim GetSemaphoreForGroup(TSubject subject)
        {
            var reducedSubject = _groupingFunc.Invoke(subject);

            if (reducedSubject == null) return null;

            var semaphore = _semaphores.GetOrAdd(reducedSubject, new SemaphoreSlim(_concurrencyLimit));

            return semaphore;
        }
    }
}
