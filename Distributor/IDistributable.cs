using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributor
{
    public interface IDistributable
    {
        Guid Id { get; }
        string ProfileName { get; }
    }
}
