using System.Collections.Generic;

namespace Delivered
{
    public interface IEndpointRepository<in TRecipient> where TRecipient : Recipient
    {
        IEnumerable<Endpoint> GetEndpointsForRecipient(TRecipient recipient);
    }
}
