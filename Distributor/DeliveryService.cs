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
                }
                catch (Exception exception)
                {
                    OnError(exception, file, endpoint);
                }
            }
        }

        protected virtual void OnError(Exception exception, DistributionFile file, TEndpoint endpoint)
        {
        }
    }
}
