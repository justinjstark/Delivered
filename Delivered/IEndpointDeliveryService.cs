using System.Threading.Tasks;

namespace Delivered
{
    public interface IEndpointDeliveryService
    {
        Task DeliverAsync(Distributable disributable, Endpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TDistributable, in TEndpoint> : IEndpointDeliveryService
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        Task DeliverAsync(TDistributable disributable, TEndpoint endpoint);
    }
}
