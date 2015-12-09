namespace Verdeler
{
    public interface IDistributor<in TDistributable, in TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        void Distribute(TDistributable distributable, TRecipient recipient);
    }
}
