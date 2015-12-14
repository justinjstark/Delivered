using System;
using System.Threading;
using System.Threading.Tasks;

namespace Verdeler
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint> : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        private SemaphoreSlim _semaphore;

        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _semaphore = new SemaphoreSlim(number);
        }

        protected abstract Task DoDeliveryAsync(TDistributable distributable, TEndpoint endpoint);

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint)
        {
            if (_semaphore != null)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
            }

            try
            {
                await DoDeliveryAsync(distributable, endpoint).ConfigureAwait(false);
            }
            finally
            {
                _semaphore?.Release();
            }
        }

        public async Task DeliverAsync(Distributable distributable, TEndpoint endpoint)
        {
            await DeliverAsync((TDistributable) distributable, endpoint).ConfigureAwait(false);
        }

        public async Task DeliverAsync(Distributable distributable, Endpoint endpoint)
        {
            await DeliverAsync((TDistributable) distributable, (TEndpoint)endpoint).ConfigureAwait(false);
        }
    }
}
