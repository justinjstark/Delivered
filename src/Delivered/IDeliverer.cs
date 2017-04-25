using System.Threading.Tasks;

namespace Delivered
{
    public interface IDeliverer
    {
        Task DeliverAsync(IDistributable distributable, IEndpoint endpoint);
    }

    public interface IDeliverer<in TDistributable, in TEndpoint> : IDeliverer
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        Task DeliverAsync(TDistributable disributable, TEndpoint endpoint);
    }
}
