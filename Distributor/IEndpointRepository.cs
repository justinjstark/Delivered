using System.Collections.Generic;

namespace Distributor
{
    public interface IEndpointRepository
    {
        IEnumerable<IEndpoint> GetEndpointsForProfile(string profileName);
    }
}
