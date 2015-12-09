using System.Collections.Generic;
using Verdeler;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointEndpointRepository : IEndpointRepository
    {
        public IEnumerable<IEndpoint> GetEndpointsForRecipient(string recipientName)
        {
            return new List<SharepointEndpoint>
            {
                new SharepointEndpoint { Uri = @"http://sharepoint.com" }
            };
        }
    }
}
