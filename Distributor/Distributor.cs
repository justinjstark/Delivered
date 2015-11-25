using System.Collections.Generic;

namespace Distributor
{
    public class Distributor<TDistributable> where TDistributable : IDistributable
    {
        public void Distribute(IEnumerable<TDistributable> distributables)
        {
            foreach (var distributable in distributables)
            {
                foreach (var endpointDeliveryService in EndpointDeliveryServices.DeliveryServices)
                {
                    endpointDeliveryService.Deliver(distributable);
                }
            }
        }
    }
}
