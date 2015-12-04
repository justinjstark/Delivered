using System.Collections.Generic;

namespace Verdeler
{
    public class Distributor<TDistributable> : IDistributor<TDistributable> where TDistributable : IDistributable
    {
        private readonly List<IDeliveryCoordinator<TDistributable>> _deliveryCoordinators
            = new List<IDeliveryCoordinator<TDistributable>>();

        public void AddEndpoint<TEndpoint>(IEndpointRepository<TEndpoint> endpointRepository, IEndpointDeliveryService<TDistributable, TEndpoint> endpointDeliveryService)
            where TEndpoint : IEndpoint
        {
            _deliveryCoordinators.Add(
                new EndpointDeliveryCoordinator<TDistributable, TEndpoint>(endpointRepository, endpointDeliveryService)
            );
        }

        public void Distribute(TDistributable distributable)
        {
            foreach (var deliveryCoordinator in _deliveryCoordinators)
            {
                deliveryCoordinator.Deliver(distributable);
            }
        }
    }
}
