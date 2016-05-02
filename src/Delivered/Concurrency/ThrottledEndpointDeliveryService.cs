using System;
using System.Threading.Tasks;
using Delivered.Concurrency.Throttlers;

namespace Delivered.Concurrency
{
    public abstract class ThrottledEndpointDeliveryService<TDistributable, TEndpoint>
        : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        private readonly MultipleGroupThrottler<TEndpoint> _throttler =
            new MultipleGroupThrottler<TEndpoint>();

        protected abstract Task DeliverThrottledAsync(TDistributable distributable, TEndpoint endpoint);

        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _throttler.AddConcurrencyLimiter(e => Guid.NewGuid(), number);
        }

        public void MaximumConcurrentDeliveries(Func<TEndpoint, object> groupingFunc, int number)
        {
            _throttler.AddConcurrencyLimiter(groupingFunc, number);
        }

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint)
        {
            await _throttler.Do(async () =>
            {
                await DeliverThrottledAsync(distributable, endpoint).ConfigureAwait(false);
            }, endpoint).ConfigureAwait(false);
        }

        public async Task DeliverAsync(IDistributable distributable, IEndpoint endpoint)
        {
            await DeliverAsync((TDistributable) distributable, (TEndpoint) endpoint).ConfigureAwait(false);
        }
    }
}
