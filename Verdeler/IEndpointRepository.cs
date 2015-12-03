using System.Collections.Generic;

namespace Verdeler
{
    public interface IEndpointRepository<out TEndpoint> where TEndpoint : Endpoint
    {
        IEnumerable<TEndpoint> GetEndpointsForRecipient(string recipientName);
    }
}
