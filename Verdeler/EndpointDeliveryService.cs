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
        private SemaphoreSlim _recipientSemaphore;

        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _semaphore = new SemaphoreSlim(number);
        }

        public void MaximumConcurrentDeliveriesPerRecipient(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _recipientSemaphore = new SemaphoreSlim(number);
        }

        protected abstract Task DoDeliveryAsync(TDistributable distributable, TEndpoint endpoint);

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint, Recipient recipient)
        {
            if (_recipientSemaphore != null)
            {
                await _recipientSemaphore.WaitAsync().ConfigureAwait(false);
            }

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
                _recipientSemaphore?.Release();
                _semaphore?.Release();
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
