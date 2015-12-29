using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private int? _maximumConcurrentDeliveriesByEndpoint;
        private Func<TEndpoint, object> _endpointConcurrencyReductionMap;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _endpointSemaphores
            = new ConcurrentDictionary<object, SemaphoreSlim>();

        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _semaphore = new SemaphoreSlim(number);
        }

        public void MaximumConcurrentDeliveries(Func<TEndpoint, object> reductionMap, int number)
        {
            _endpointConcurrencyReductionMap = reductionMap;
            _maximumConcurrentDeliveriesByEndpoint = number;
        }

        public abstract Task DoDeliveryAsync(TDistributable distributable, TEndpoint endpoint);

        public async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint, Recipient recipient)
        {
            var endpointSemaphore = GetEndpointSemaphore(endpoint);
            
            await Task.WhenAll(
                endpointSemaphore?.WaitAsync() ?? Task.FromResult(0),
                _semaphore?.WaitAsync() ?? Task.FromResult(0)).ConfigureAwait(false);

            try
            {
                await DoDeliveryAsync(distributable, endpoint).ConfigureAwait(false);
            }
            finally
            {
                endpointSemaphore?.Release();
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

        private SemaphoreSlim GetEndpointSemaphore(TEndpoint endpoint)
        {
            if (_maximumConcurrentDeliveriesByEndpoint == null)
            {
                return null;
            }

            var reduced = _endpointConcurrencyReductionMap.Invoke(endpoint);

            var semaphore = _endpointSemaphores.GetOrAdd(reduced,
                new SemaphoreSlim(_maximumConcurrentDeliveriesByEndpoint.Value));

            return semaphore;
        }
    }
}
