namespace Verdeler
{
    public interface IEndpointDeliveryService
    {
        void Deliver(IDistributable distributable);
    }
}
