namespace Verdeler
{
    public interface IEndpointDeliveryService<in TDistributable, in TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        void Deliver(TDistributable disributable, TEndpoint endpoint);
    }
}
