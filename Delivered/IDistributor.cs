using System.Threading.Tasks;

namespace Delivered
{
    public interface IDistributor<in TDistributable, in TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        Task DistributeAsync(TDistributable distributable, TRecipient recipient);
    }
}
