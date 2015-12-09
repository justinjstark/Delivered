namespace Verdeler
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint> : IEndpointDeliveryService<TDistributable, TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        public abstract void Deliver(TDistributable distributable, TEndpoint endpiont);

        public void Deliver(IDistributable distributable, TEndpoint endpoint)
        {
            Deliver((TDistributable) distributable, endpoint);
        }

        public void Deliver(IDistributable distributable, IEndpoint endpoint)
        {
            Deliver((TDistributable)distributable, (TEndpoint)endpoint);
        }
    }
}
