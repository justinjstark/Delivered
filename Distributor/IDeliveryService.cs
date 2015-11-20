namespace Distributor
{
    public interface IDeliveryService
    {
        void DeliverToEndpoints(File file, IEndpoint endpoint);
    }
}
