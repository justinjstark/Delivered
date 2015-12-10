using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Verdeler
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint>
        : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        private SemaphoreSlim _semaphore;
        private int _maximumConcurrentDeliveriesPerRecipient;
        private ConcurrentDictionary<Recipient, SemaphoreSlim> _recipientSemaphores
            = new ConcurrentDictionary<Recipient, SemaphoreSlim>();

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

            _maximumConcurrentDeliveriesPerRecipient = number;
        }

        protected abstract Task DoDeliveryAsync(TDistributable distributable, TEndpoint endpoint);

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint, Recipient recipient)
        {
            var recipientSemaphore = GetRecipientSemaphore(recipient);

            if (recipientSemaphore != null)
            {
                await recipientSemaphore.WaitAsync().ConfigureAwait(false);
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
                recipientSemaphore?.Release();
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

        private SemaphoreSlim GetRecipientSemaphore(Recipient recipient)
        {
            return _recipientSemaphores.GetOrAdd(recipient, new SemaphoreSlim(_maximumConcurrentDeliveriesPerRecipient));
        }
    }
}
