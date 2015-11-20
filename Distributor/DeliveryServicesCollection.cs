using System.Collections;
using System.Collections.Generic;

namespace Distributor
{
    public sealed class DeliveryServicesCollection : IEnumerable<IDeliveryService>
    {
        private readonly List<IDeliveryService> _distributionMethods = new List<IDeliveryService>();

        public void Add(IDeliveryService distributor)
        {
            _distributionMethods.Add(distributor);
        }

        public IEnumerator<IDeliveryService> GetEnumerator()
        {
            return _distributionMethods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
