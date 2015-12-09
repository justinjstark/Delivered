using System.Collections.Generic;

namespace Verdeler
{
    public interface IEndpointRepository
    {
        IEnumerable<IEndpoint> GetEndpointsForRecipient(string recipientName);
    }
}
