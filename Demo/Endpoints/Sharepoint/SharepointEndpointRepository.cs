using System.Collections.Generic;
using Distributor;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointEndpointRepository : IEndpointRepository<SharepointEndpoint>
    {
        public IEnumerable<SharepointEndpoint> GetEndpointsForRecipient(string recipientName)
        {
            return new List<SharepointEndpoint>
            {
                new SharepointEndpoint { Uri = @"http://sharepoint.com" }
            };
        }
    }
}
