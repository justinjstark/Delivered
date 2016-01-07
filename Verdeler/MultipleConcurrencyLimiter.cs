using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Verdeler
{
    internal class MultipleConcurrencyLimiter<TSubject>
    {
        private readonly List<ConcurrencyLimiter<TSubject>> _concurrencyLimiters =
            new List<ConcurrencyLimiter<TSubject>>();

        public void AddConcurrencyLimiter(Func<TSubject, object> reductionMap, int number)
        {
            _concurrencyLimiters.Add(new ConcurrencyLimiter<TSubject>(reductionMap, number));
        }

        public async Task Do(Func<Task> asyncFunc, TSubject subject)
        {
            //We must obtain locks in order to prevent cycles from forming.
            foreach (var concurrencyLimiter in _concurrencyLimiters)
            {
                await concurrencyLimiter.WaitFor(subject).ConfigureAwait(false);
            }

            try
            {
                await asyncFunc().ConfigureAwait(false);
            }
            finally
            {
                //Release in reverse order
                _concurrencyLimiters
                    .AsEnumerable().Reverse().ToList()
                    .ForEach(l => l.Release(subject));
            }
        }
    }
}
