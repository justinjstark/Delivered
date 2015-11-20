namespace Distributor
{
    public class DistributionMethod
    {
        private readonly IEndpointRepository _endpointRepository;
        private readonly IDeliveryService _deliveryService;

        public DistributionMethod(IEndpointRepository endpointRepository, IDeliveryService deliveryService)
        {
            _endpointRepository = endpointRepository;
            _deliveryService = deliveryService;
        }

        public void DeliverToEndpoints(File file, string profileName)
        {
            var endpoints = _endpointRepository.GetEndpointsForProfile("");

            foreach (var endpoint in endpoints)
            {
                _deliveryService.DeliverToEndpoints(file, endpoint);
            }
        }
    }
}
