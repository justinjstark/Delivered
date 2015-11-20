namespace Distributor
{
    public abstract class DeliveryService<TEndpoint> : IDeliveryService where TEndpoint : IEndpoint
    {
        protected abstract void DeliverFileToEndpoint(DistributionFile file, TEndpoint endpoint);

        private readonly IEndpointRepository<TEndpoint> _endpointRepository;

        protected DeliveryService(IEndpointRepository<TEndpoint> endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        public void DeliverFile(DistributionFile file)
        {
            var endpoints = _endpointRepository.GetEndpointsForProfile(file.ProfileName);

            foreach (var endpoint in endpoints)
            {
                DeliverFileToEndpoint(file, endpoint);
            }
        }
    }
}
