using System;
using System.Collections.Generic;
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
            //Linq doesn't play well with async/await.
            //This does NOT work if converted to a .Select and a Task.WhenAll();
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
                _concurrencyLimiters.ForEach(l => l.Release(subject));
            }
        }
    }
}
