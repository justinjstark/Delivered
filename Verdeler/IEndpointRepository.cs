using System.Collections.Generic;

namespace Verdeler
{
    public interface IEndpointRepository<out TEndpoint> where TEndpoint : IEndpoint
    {
        IEnumerable<TEndpoint> GetEndpointsForRecipient(string recipientName);
    }
}
