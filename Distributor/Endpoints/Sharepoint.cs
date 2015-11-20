using System;
using System.Collections.Generic;

namespace Distributor.Endpoints
{
    public class SharepointEndpoint : IEndpoint
    {
        public string Uri { get; set; }
    }

    public class SharepointEndpointRepository : IEndpointRepository
    {
        public IEnumerable<IEndpoint> GetEndpointsForProfile(string profileName)
        {
            return new List<IEndpoint>
            {
                new SharepointEndpoint { Uri = "site1" },
                new SharepointEndpoint { Uri = "site2" }
            };
        }
    }

    public class SharepointDeliveryService : IDeliveryService
    {
        public void DeliverToEndpoints(File file, IEndpoint endpoint)
        {
            var sharepointEndpoint = (SharepointEndpoint)endpoint;

            Console.WriteLine($"Delivering file {file.Name} to SharePoint URI {sharepointEndpoint.Uri}");
        }
    }
}
