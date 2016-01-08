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

        public void AddConcurrencyLimiter(Func<TSubject, object> groupingFunc, int number)
        {
            _concurrencyLimiters.Add(new ConcurrencyLimiter<TSubject>(groupingFunc, number));
        }

        public async Task Do(Func<Task> asyncFunc, TSubject subject)
        {
            //Track which concurrency limiters have been entered in case of exception
            var concurrencyLimitersEntered = new List<ConcurrencyLimiter<TSubject>>();

            try
            {
                //We must obtain locks in order to prevent cycles from forming
                foreach (var concurrencyLimiter in _concurrencyLimiters)
                {
                    var task = concurrencyLimiter.WaitFor(subject);

                    concurrencyLimitersEntered.Add(concurrencyLimiter);

                    await task.ConfigureAwait(false);
                }

                await asyncFunc().ConfigureAwait(false);
            }
            finally
            {
                //Release in reverse order
                concurrencyLimitersEntered
                    .AsEnumerable().Reverse().ToList()
                    .ForEach(l => l.Release(subject));
            }
        }
    }
}
