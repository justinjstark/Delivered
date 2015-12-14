using System.Threading.Tasks;

namespace Verdeler
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint> : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : Distributable
        where TEndpoint : Endpoint
    {
        public abstract Task DeliverAsync(TDistributable distributable, TEndpoint endpiont);

        public async Task DeliverAsync(Distributable distributable, TEndpoint endpoint)
        {
            await DeliverAsync((TDistributable) distributable, endpoint);
        }

        public async Task DeliverAsync(Distributable distributable, Endpoint endpoint)
        {
            await DeliverAsync((TDistributable) distributable, (TEndpoint)endpoint);
        }
    }
}
