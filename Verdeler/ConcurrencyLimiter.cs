using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Verdeler
{
    internal class ConcurrencyLimiter<TSubject>
    {
        private readonly Func<TSubject, object> _subjectReductionMap;

        private readonly int _concurrencyLimit;

        private readonly ConcurrentDictionary<object, SemaphoreSlim> _semaphores
            = new ConcurrentDictionary<object, SemaphoreSlim>();

        public ConcurrencyLimiter(Func<TSubject, object> subjectReductionMap, int concurrencyLimit)
        {
            if (concurrencyLimit <= 0)
            {
                throw new ArgumentException(@"Concurrency limit must be greater than 0.", nameof(concurrencyLimit));
            }

            _subjectReductionMap = subjectReductionMap;
            _concurrencyLimit = concurrencyLimit;
        }

        public async Task WaitFor(TSubject subject)
        {
            var semaphore = GetSemaphoreForReduction(subject);

            await semaphore.WaitAsync();
        }

        public void Release(TSubject subject)
        {
            var semaphore = GetSemaphoreForReduction(subject);

            semaphore.Release();
        }

        private SemaphoreSlim GetSemaphoreForReduction(TSubject subject)
        {
            var reducedSubject = _subjectReductionMap.Invoke(subject);

            var semaphore = _semaphores.GetOrAdd(reducedSubject, new SemaphoreSlim(_concurrencyLimit));

            return semaphore;
        }
    }
}
