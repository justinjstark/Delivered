using System.Collections.Generic;
using System.Threading.Tasks;

namespace Verdeler
{
    public interface IEndpointRepository<in TRecipient> where TRecipient : Recipient
    {
        IEnumerable<Endpoint> GetEndpointsForRecipient(TRecipient recipient);
    }
}
