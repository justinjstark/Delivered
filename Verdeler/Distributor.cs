using System;
using System.Collections.Generic;
using System.Linq;

namespace Verdeler
{
    public class Distributor<TDistributable> : IDistributor<TDistributable> where TDistributable : IDistributable
    {
        private readonly List<IEndpointRepository> _endpointRepositories
            = new List<IEndpointRepository>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

        public void RegisterEndpointRepository(IEndpointRepository endpointRepository)
        {
            if (!_endpointRepositories.Contains(endpointRepository))
            {
                _endpointRepositories.Add(endpointRepository);
            }
        }

        public void RegisterEndpointDeliveryService<TEndpoint>(IEndpointDeliveryService<TEndpoint> endpointDeliveryService)
        {
            _endpointDeliveryServices[typeof(TEndpoint)] = endpointDeliveryService;
        }

        public void Distribute(TDistributable distributable, string recipientName)
        {
            var endpoints = _endpointRepositories.SelectMany(r => r.GetEndpointsForRecipient(recipientName));

            foreach (var endpoint in endpoints)
            {
                var endpointDeliveryService = _endpointDeliveryServices[endpoint.GetType()];

                endpointDeliveryService.Deliver(distributable, endpoint);
            }
        }
    }
}
