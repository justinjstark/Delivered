namespace Verdeler
{
    internal class EndpointDeliveryCoordinator<TDistributable, TEndpoint> : IDeliveryCoordinator<TDistributable>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        private readonly IEndpointRepository<TEndpoint> _endpointRepository; 
        private readonly IEndpointDeliveryService<TDistributable, TEndpoint> _endpointDeliveryService;

        public EndpointDeliveryCoordinator(IEndpointRepository<TEndpoint> endpointRepository, IEndpointDeliveryService<TDistributable, TEndpoint> endpointDeliveryService)
        {
            _endpointRepository = endpointRepository;
            _endpointDeliveryService = endpointDeliveryService;
        } 

        public void Deliver(TDistributable distributable)
        {
            var endpoints = _endpointRepository.GetEndpointsForRecipient(distributable.RecipientName);

            foreach (var endpoint in endpoints)
            {
                _endpointDeliveryService.Deliver(distributable, endpoint);
            }
        }
    }
}
