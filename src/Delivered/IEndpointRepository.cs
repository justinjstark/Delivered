using System.Collections.Generic;

namespace Delivered
{
    public interface IEndpointRepository<in TRecipient> where TRecipient : IRecipient
    {
        IEnumerable<IEndpoint> GetEndpointsForRecipient(TRecipient recipient);
    }
}
