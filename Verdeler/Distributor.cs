using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Verdeler
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        public Concurrency Concurrency;

        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

        public Distributor()
        {
            Concurrency = Concurrency.Asynchronous;
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

            await Task.WhenAll(deliveryTasks);
        }

        public async Task DeliverAsync(IEndpointDeliveryService endpointDeliveryService, TDistributable distributable, Endpoint endpoint)
        {
            var deliveryTask = endpointDeliveryService.DeliverAsync(distributable, endpoint);

            if (Concurrency == Concurrency.Synchronous)
            {
                deliveryTask.Wait();
            }

            await deliveryTask;
        }
    }
}
