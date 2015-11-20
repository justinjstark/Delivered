using System;

namespace Distributor
{
    public interface IDeliveryService
    {
        void DeliverFile(DistributionFile file);
    }
}
