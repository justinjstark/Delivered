using System;
using System.Threading.Tasks;

namespace Delivered
{
    public abstract class ConcurrencyLimitedEndpointDeliveryService<TDistributable, TEndpoint>
        : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        private readonly MultipleConcurrencyLimiter<TEndpoint> _multipleConcurrencyLimiter =
            new MultipleConcurrencyLimiter<TEndpoint>();
        
        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _multipleConcurrencyLimiter.AddConcurrencyLimiter(e => 1842872753, number);
        }

        public void MaximumConcurrentDeliveries(Func<TEndpoint, object> groupingFunc, int number)
        {
            _multipleConcurrencyLimiter.AddConcurrencyLimiter(groupingFunc, number);
        }

        protected abstract Task DoDeliveryAsync(TDistributable distributable, TEndpoint endpoint);

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint)
        {
            await _multipleConcurrencyLimiter.Do(async () =>
            {
                await DoDeliveryAsync(distributable, endpoint).ConfigureAwait(false);
            }, endpoint);
        }

        public async Task DeliverAsync(IDistributable distributable, IEndpoint endpoint)
        {
            await DeliverAsync((TDistributable) distributable, (TEndpoint) endpoint).ConfigureAwait(false);
        }
    }
}
