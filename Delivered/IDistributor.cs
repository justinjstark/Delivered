using System.Threading.Tasks;

namespace Delivered
{
    public interface IDistributor<in TDistributable, in TRecipient>
        where TDistributable : IDistributable
        where TRecipient : IRecipient
    {
        Task DistributeAsync(TDistributable distributable, TRecipient recipient);
    }
}
