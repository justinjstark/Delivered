using System.Collections.Generic;

namespace Verdeler
{
    public interface IDistributor<in TDistributable> where TDistributable : IDistributable
    {
        void Distribute(TDistributable distributable);
    }
}
