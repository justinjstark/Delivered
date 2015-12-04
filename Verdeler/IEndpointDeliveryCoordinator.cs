namespace Verdeler
{
    internal interface IEndpointDeliveryCoordinator<in TDistributable>
        where TDistributable : IDistributable
    {
        void Deliver(TDistributable distributable);
    }
}
