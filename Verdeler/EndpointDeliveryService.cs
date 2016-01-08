using System;
using System.Threading.Tasks;

namespace Verdeler
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint>
        : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        private readonly MultipleConcurrencyLimiter<TEndpoint> _multipleConcurrencyLimiter =
            new MultipleConcurrencyLimiter<TEndpoint>();
        
        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _multipleConcurrencyLimiter.AddConcurrencyLimiter(e => 184204872305603, number);
        }

        public void MaximumConcurrentDeliveries(Func<TEndpoint, object> groupingFunc, int number)
        {
            _multipleConcurrencyLimiter.AddConcurrencyLimiter(groupingFunc, number);
        }

        public abstract Task DoDeliveryAsync(TDistributable distributable, TEndpoint endpoint);

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint, Recipient recipient)
        {
            await _multipleConcurrencyLimiter.Do(async () =>
            {
                await DoDeliveryAsync(distributable, endpoint).ConfigureAwait(false);
            }, endpoint);
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
