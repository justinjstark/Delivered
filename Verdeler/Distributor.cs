using System.Collections.Generic;

namespace Verdeler
{
    public class Distributor<TDistributable> : IDistributor<TDistributable> where TDistributable : IDistributable
    {
        public readonly EndpointDeliveryServicesCollection EndpointDeliveryServices = new EndpointDeliveryServicesCollection();
        
        public void Distribute(TDistributable distributable)
        {
            foreach (var endpointDeliveryService in EndpointDeliveryServices)
            {
                endpointDeliveryService.Deliver(distributable);
            }
        }
    }
}
