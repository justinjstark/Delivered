namespace Verdeler
{
    public interface IDistributor<in TDistributable, in TRecipient>
        where TDistributable : IDistributable
        where TRecipient : IRecipient
    {
        void Distribute(TDistributable distributable, TRecipient recipient);
    }
}
