using System;
using System.Collections.Generic;
using Distributor;

namespace Demo.Endpoints
{
    public class SharepointEndpoint : IEndpoint
    {
        public Guid Id { get; set; }
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

        protected override void DeliverFileToEndpoint(DistributionFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.FileName} to Sharepoint URI {endpoint.Uri}");
        }

        protected override void OnError(Exception exception, DistributionFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Error distributing file {file.FileName} to Sharepoint URI {endpoint.Uri}");
        }
    }
}
