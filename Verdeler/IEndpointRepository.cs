using System.Collections.Generic;

namespace Verdeler
{
    public interface IEndpointRepository<in TRecipient> where TRecipient : Recipient
    {
        IEnumerable<Endpoint> GetEndpointsForRecipient(TRecipient recipient);
    }
}
