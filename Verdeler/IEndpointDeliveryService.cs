namespace Verdeler
{
    public interface IEndpointDeliveryService
    {
        void Deliver(IDistributable distributable, IEndpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TDistributable, in TEndpoint> : IEndpointDeliveryService
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        void Deliver(TDistributable disributable, TEndpoint endpoint);
    }
}
