using System.Threading.Tasks;

namespace Verdeler
{
    public interface IEndpointDeliveryService
    {
        void Deliver(Distributable distributable, Endpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TEndpoint> : IEndpointDeliveryService
    {
        void Deliver(Distributable distributable, TEndpoint endpoint);
    }

    public interface IEndpointDeliveryService<in TDistributable, in TEndpoint> : IEndpointDeliveryService<TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        void Deliver(TDistributable disributable, TEndpoint endpoint);
    }
}
