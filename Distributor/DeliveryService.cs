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

        protected abstract void Deliver(Delivery<TEndpoint> delivery);
        
        public void DeliverFile(DistributionFile file)
        {
            var endpoints = _endpointRepository.GetEndpointsForProfile(file.ProfileName);

            foreach (var endpoint in endpoints)
            {
                var delivery = new Delivery<TEndpoint>
                {
                    File = file,
                    Endpoint = endpoint
                };

                try
                {
                    Deliver(delivery);

                    OnSuccess(delivery);

                    //TODO: Mark file as delivered to endpoint.
                    //Not all deliveries may be idempotent so we only ever want to deliver a file once.
                }
                catch (Exception exception)
                {
                    //Call virtual OnError method and then continue to the next endpoint delivery.
                    OnError(delivery, exception);
                }
            }
        }

        protected virtual void OnSuccess(Delivery<TEndpoint> delivery)
        {
        }

        protected virtual void OnError(Delivery<TEndpoint> delivery, Exception exception)
        {
        }
    }
}
