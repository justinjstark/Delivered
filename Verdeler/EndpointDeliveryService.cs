using System.Threading.Tasks;

namespace Verdeler
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint> : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        public abstract void Deliver(TDistributable distributable, TEndpoint endpiont);

        public void Deliver(Distributable distributable, TEndpoint endpoint)
        {
            Deliver((TDistributable) distributable, endpoint);
        }

        public void Deliver(Distributable distributable, Endpoint endpoint)
        {
            Deliver((TDistributable)distributable, (TEndpoint)endpoint);
        }
    }
}
