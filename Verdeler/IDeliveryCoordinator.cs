namespace Verdeler
{
    internal interface IDeliveryCoordinator<in TDistributable>
        where TDistributable : IDistributable
    {
        void Deliver(TDistributable distributable);
    }
}
