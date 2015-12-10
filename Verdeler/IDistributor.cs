using System.Threading.Tasks;

namespace Verdeler
{
    public interface IDistributor<in TDistributable, in TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        Task Distribute(TDistributable distributable, TRecipient recipient);
    }
}
