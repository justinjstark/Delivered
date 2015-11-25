using System.Collections;
using System.Collections.Generic;

namespace Distributor
{
    public sealed class EndpointDeliveryServicesCollection : IEnumerable<IEndpointDeliveryService>
    {
        private readonly List<IEndpointDeliveryService> _endpointDeliveryServices = new List<IEndpointDeliveryService>();

        public void Add(IEndpointDeliveryService endpointDeliveryService)
        {
            _endpointDeliveryServices.Add(endpointDeliveryService);
        }

        public IEnumerator<IEndpointDeliveryService> GetEnumerator()
        {
            return _endpointDeliveryServices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
