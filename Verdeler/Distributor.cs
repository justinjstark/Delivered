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
        public int MaxConcurrentDeliveries;

        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

        public Distributor()
        {
            MaxConcurrentDeliveries = 10;
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
            var throttler = new SemaphoreSlim(MaxConcurrentDeliveries);

            var endpoints = _endpointRepositories
                .SelectMany(r => r.GetEndpointsForRecipient(recipient));

            var deliveryTasks = endpoints
                .Select(e => new {Endpoint = e, EndpointDeliveryService = _endpointDeliveryServices[e.GetType()]})
                .Select(e => DeliverAsync(throttler, e.EndpointDeliveryService, distributable, e.Endpoint));

            await Task.WhenAll(deliveryTasks);
        }

        public async Task DeliverAsync(SemaphoreSlim throttler, IEndpointDeliveryService endpointDeliveryService,
            TDistributable distributable, Endpoint endpoint)
        {
            await throttler.WaitAsync();

            try
            {
                await endpointDeliveryService.DeliverAsync(distributable, endpoint);
            }
            finally
            {
                throttler.Release();
            }
        }
    }
}
