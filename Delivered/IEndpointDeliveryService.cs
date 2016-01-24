using System.Threading.Tasks;

namespace Delivered
{
    public interface IEndpointDeliveryService
    {
        Task DeliverAsync(IDistributable disributable, IEndpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TDistributable, in TEndpoint> : IEndpointDeliveryService
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        Task DeliverAsync(TDistributable disributable, TEndpoint endpoint);
    }
}
