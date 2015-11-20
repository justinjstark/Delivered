using System;
using System.Collections.Generic;

namespace Distributor.Endpoints
{
    public class SharepointEndpoint : IEndpoint
    {
        public string Uri { get; set; }
    }

    public class SharepointEndpointRepository : IEndpointRepository<SharepointEndpoint>
    {
        public IEnumerable<SharepointEndpoint> GetEndpointsForProfile(string profileName)
        {
            return new List<SharepointEndpoint>
            {
                new SharepointEndpoint { Uri = "site1" },
                new SharepointEndpoint { Uri = "site2" }
            };
        }
    }

    public class SharepointDeliveryService : DeliveryService<SharepointEndpoint>
    {
        public SharepointDeliveryService(SharepointEndpointRepository endpointRepository) : base(endpointRepository)
        {
        }

        protected override void DeliverFileToEndpoint(File file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");
        }
    }
}
