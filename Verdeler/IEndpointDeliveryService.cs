using System.Threading.Tasks;

namespace Verdeler
{
    public interface IEndpointDeliveryService
    {
        Task DeliverAsync(Distributable distributable, Endpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TEndpoint> : IEndpointDeliveryService
    {
        Task DeliverAsync(Distributable distributable, TEndpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TDistributable, in TEndpoint> : IEndpointDeliveryService<TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        Task DeliverAsync(TDistributable disributable, TEndpoint endpoint);
    }
}
