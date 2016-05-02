using System;
using System.Threading.Tasks;
using Delivered.Concurrency.Throttlers;

namespace Delivered
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint>
        : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        private readonly MultipleGroupThrottler<TEndpoint> _throttler =
            new MultipleGroupThrottler<TEndpoint>();

        public abstract Task DeliverAsync(TDistributable distributable, TEndpoint endpoint);

        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _throttler.AddConcurrencyLimiter(e => 0, number);
        }

        public void MaximumConcurrentDeliveries(Func<TEndpoint, object> groupingFunc, int number)
        {
            _throttler.AddConcurrencyLimiter(groupingFunc, number);
        }

        async Task IEndpointDeliveryService<TDistributable, TEndpoint>.DeliverAsync(TDistributable distributable, TEndpoint endpoint)
        {
            await DeliverWithThrottlingAsync(distributable, endpoint).ConfigureAwait(false);
        }

        async Task IEndpointDeliveryService.DeliverAsync(IDistributable distributable, IEndpoint endpoint)
        {
            await DeliverWithThrottlingAsync((TDistributable)distributable, (TEndpoint)endpoint).ConfigureAwait(false);
        }

        private async Task DeliverWithThrottlingAsync(TDistributable distributable, TEndpoint endpoint)
        {
            await _throttler.Do(async () =>
            {
                await DeliverAsync(distributable, endpoint).ConfigureAwait(false);
            }, endpoint).ConfigureAwait(false);
        }
    }
}
