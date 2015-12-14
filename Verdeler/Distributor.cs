using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Verdeler
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        public Concurrency DeliveryConcurrency;

        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

        public Distributor()
        {
            DeliveryConcurrency = Concurrency.Asynchronous;
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
                .Select(r => r.GetEndpointsForRecipient(recipient));

            var deliveryTasks = endpoints
                .SelectMany(e => DeliverToEndpoints(e, distributable));

            await Task.WhenAll(deliveryTasks);
        }

        private IEnumerable<Task> DeliverToEndpoints(IEnumerable<Endpoint> endpoints, TDistributable distributable)
        {
            return endpoints
                .Select(e => new {EndpointDeliveryService = _endpointDeliveryServices[e.GetType()], Endpoint = e})
                .Select(e => DeliverAsync(e.EndpointDeliveryService, distributable, e.Endpoint));
        }

        private Task DeliverAsync(IEndpointDeliveryService endpointDeliveryService, TDistributable distributable, Endpoint endpoint)
        {
            var task = new Task(() => endpointDeliveryService.DeliverAsync(distributable, endpoint));

            switch (DeliveryConcurrency)
            {
                case Concurrency.Asynchronous:
                    task.Start();
                    break;
                case Concurrency.Synchronous:
                    task.RunSynchronously();
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(DeliveryConcurrency));
            }

            return task;
        }
    }
}
