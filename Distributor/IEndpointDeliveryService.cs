namespace Distributor
{
    public interface IEndpointDeliveryService
    {
        void DeliverFile(DistributionFile file);
    }
}
