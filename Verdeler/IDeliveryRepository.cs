using System;
using System.Collections.Generic;

namespace Verdeler
{
    public interface IDeliveryRepository
    {
        IEnumerable<Delivery> GetDeliveries(Guid distributableId, Guid endpointId);
        void RecordDelivery(Delivery delivery);
    }
}
