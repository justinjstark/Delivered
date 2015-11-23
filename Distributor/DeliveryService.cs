using System;
using System.Linq;

namespace Distributor
{
    public abstract class DeliveryService<TEndpoint> : IDeliveryService where TEndpoint : IEndpoint
    {
        private readonly IEndpointRepository<TEndpoint> _endpointRepository;
        
        protected DeliveryService(IEndpointRepository<TEndpoint> endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        protected abstract void Deliver(DistributionFile file, TEndpoint endpoint);
        
        public void DeliverFile(DistributionFile file)
        {
            var endpoints = _endpointRepository.GetEndpointsForProfile(file.ProfileName);

            foreach (var endpoint in endpoints)
            {
                try
                {
                    Deliver(file, endpoint);

                    OnSuccess(file, endpoint);

                    //TODO: Mark file as delivered to endpoint.
                    //Not all deliveries may be idempotent so we only ever want to deliver a file once.
                }
                catch (Exception exception)
                {
                    //Call virtual OnError method and then continue to the next endpoint delivery.
                    OnError(file, endpoint, exception);
                }
            }
        }

        protected virtual void OnSuccess(DistributionFile file, TEndpoint endpoint)
        {
        }

        protected virtual void OnError(DistributionFile file, TEndpoint endpoint, Exception exception)
        {
        }
    }
}
