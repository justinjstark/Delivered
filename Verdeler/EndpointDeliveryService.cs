using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Verdeler
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint>
        : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        private readonly ConcurrencyLimiter<TEndpoint> _concurrencyLimiter;

        private readonly List<ConcurrencyLimiter<TEndpoint>> _concurrencyLimiters =
            new List<ConcurrencyLimiter<TEndpoint>>();
        
        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _concurrencyLimiters.Add(new ConcurrencyLimiter<TEndpoint>(e => 184204872305603, number));
        }

        public void MaximumConcurrentDeliveries(Func<TEndpoint, object> reductionMap, int number)
        {
            _concurrencyLimiters.Add(new ConcurrencyLimiter<TEndpoint>(reductionMap, number));
        }

        public abstract Task DoDeliveryAsync(TDistributable distributable, TEndpoint endpoint);

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint, Recipient recipient)
        {
            await Task.WhenAll(_concurrencyLimiters.Select(l => l.WaitFor(endpoint)));

            try
            {
                await DoDeliveryAsync(distributable, endpoint).ConfigureAwait(false);
            }
            finally
            {
                _concurrencyLimiters.ForEach(l => l.Release(endpoint));
            }
        }

        public async Task DeliverAsync(Distributable distributable, TEndpoint endpoint, Recipient recipient)
        {
            await DeliverAsync((TDistributable) distributable, endpoint, recipient).ConfigureAwait(false);
        }

        public async Task DeliverAsync(Distributable distributable, Endpoint endpoint, Recipient recipient)
        {
            await DeliverAsync((TDistributable) distributable, (TEndpoint) endpoint, recipient).ConfigureAwait(false);
        }
    }
}
