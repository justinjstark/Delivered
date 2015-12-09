using System.Collections.Generic;

namespace Verdeler
{
    public interface IEndpointRepository<in TRecipient> where TRecipient : IRecipient
    {
        IEnumerable<IEndpoint> GetEndpointsForRecipient(TRecipient recipient);
    }
}
