using System;

namespace Distributor
{
    public abstract class DeliveryService<TEndpoint> : IDeliveryService where TEndpoint : IEndpoint
    {
        private readonly IEndpointRepository<TEndpoint> _endpointRepository;
        
        protected DeliveryService(IEndpointRepository<TEndpoint> endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        protected abstract void DeliverFileToEndpoint(DistributionFile file, TEndpoint endpoint);

        public void DeliverFile(DistributionFile file)
        {
            var endpoints = _endpointRepository.GetEndpointsForProfile(file.ProfileName);

            foreach (var endpoint in endpoints)
            {
                try
                {
                    DeliverFileToEndpoint(file, endpoint);

                    OnSuccess(file, endpoint);

                    //TODO: Mark file as delivered to endpoint.
                    //Not all deliveries may be idempotent so we only ever want to deliver a file once.
                }
                catch (Exception exception)
                {
                    //Call virtual OnError method and then continue to the next endpoint delivery.
                    OnError(exception, file, endpoint);
                }
            }
        }

        protected virtual void OnSuccess(DistributionFile file, TEndpoint endpoint)
        {
        }

        protected virtual void OnError(Exception exception, DistributionFile file, TEndpoint endpoint)
        {
        }
    }
}
