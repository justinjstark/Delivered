namespace Verdeler
{
    public interface IEndpointDeliveryService
    {
        void Deliver(IDistributable distributable, IEndpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TEndpoint> : IEndpointDeliveryService
    {
        void Deliver(IDistributable distributable, TEndpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TDistributable, in TEndpoint> : IEndpointDeliveryService<TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        void Deliver(TDistributable disributable, TEndpoint endpoint);
    }
}
