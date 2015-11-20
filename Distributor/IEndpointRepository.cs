using System.Collections.Generic;

namespace Distributor
{
    public interface IEndpointRepository<out TEndpoint> where TEndpoint : IEndpoint
    {
        IEnumerable<TEndpoint> GetEndpointsForProfile(string profileName);
    }
}
