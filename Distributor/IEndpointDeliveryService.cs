namespace Distributor
{
    public interface IEndpointDeliveryService
    {
        void Deliver(IDistributable distributable);
    }
}
