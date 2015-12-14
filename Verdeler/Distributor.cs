using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Verdeler
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        private SemaphoreSlim _semaphore;

        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, SemaphoreSlim> _endpointThrottlers
            = new Dictionary<Type, SemaphoreSlim>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _semaphore = new SemaphoreSlim(number);
        }

        public void RegisterEndpointRepository(IEndpointRepository<TRecipient> endpointRepository)
        {
            if (!_endpointRepositories.Contains(endpointRepository))
            {
                _endpointRepositories.Add(endpointRepository);
            }
        }

        public void RegisterEndpointDeliveryService<TEndpoint>(IEndpointDeliveryService<TEndpoint> endpointDeliveryService)
            where TEndpoint : Endpoint
        {
            _endpointDeliveryServices[typeof(TEndpoint)] = endpointDeliveryService;
        }

        public async Task DistributeAsync(TDistributable distributable, TRecipient recipient)
        {
            var endpoints = _endpointRepositories
                .SelectMany(r => r.GetEndpointsForRecipient(recipient));

            var deliveryTasks = endpoints
                .Select(e => new {Endpoint = e, EndpointDeliveryService = _endpointDeliveryServices[e.GetType()]})
                .Select(e => DeliverAsync(e.EndpointDeliveryService, distributable, e.Endpoint));

            await Task.WhenAll(deliveryTasks).ConfigureAwait(false);
        }

        public async Task DeliverAsync(IEndpointDeliveryService endpointDeliveryService,
            TDistributable distributable, Endpoint endpoint)
        {
            if (_semaphore != null)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
            }

            try
            {
                await endpointDeliveryService.DeliverAsync(distributable, endpoint).ConfigureAwait(false);
            }
            finally
            {
                _semaphore?.Release();
            }
        }
    }
}
