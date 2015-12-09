using System;
using System.Collections.Generic;
using System.Linq;

namespace Verdeler
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

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

        public void Distribute(TDistributable distributable, TRecipient recipient)
        {
            var endpoints = _endpointRepositories.SelectMany(r => r.GetEndpointsForRecipient(recipient));

            foreach (var endpoint in endpoints)
            {
                var endpointDeliveryService = _endpointDeliveryServices[endpoint.GetType()];

                endpointDeliveryService.Deliver(distributable, endpoint);
            }
        }
    }
}
