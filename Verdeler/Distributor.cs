using System.Collections.Generic;

namespace Verdeler
{
    public class Distributor<TDistributable> : IDistributor<TDistributable> where TDistributable : IDistributable
    {
        private readonly List<IEndpointDeliveryCoordinator<TDistributable>> _endpointDeliveryCoordinators
            = new List<IEndpointDeliveryCoordinator<TDistributable>>();

        public void AddEndpoint<TEndpoint>(IEndpointRepository<TEndpoint> endpointRepository, IEndpointDeliveryService<TDistributable, TEndpoint> endpointDeliveryService)
            where TEndpoint : IEndpoint
        {
            _endpointDeliveryCoordinators.Add(
                new EndpointDeliveryCoordinator<TDistributable, TEndpoint>(endpointRepository, endpointDeliveryService)
            );
        }

        public void Distribute(TDistributable distributable)
        {
            foreach (var endpointDeliveryCoordinator in _endpointDeliveryCoordinators)
            {
                endpointDeliveryCoordinator.Deliver(distributable);
            }
        }
    }
}
