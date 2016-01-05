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
            //NOTE: This cannot be replaced with a Linq .ForEach
            foreach (var cl in _concurrencyLimiters)
            {
                await cl.WaitFor(subject);
            }

            try
            {
                await asyncFunc().ConfigureAwait(false);
            }
            finally
            {
                //Release in the reverse order to prevent deadlocks
                _concurrencyLimiters
                    .AsEnumerable().Reverse().ToList()
                    .ForEach(l => l.Release(subject));
            }
        }
    }
}
