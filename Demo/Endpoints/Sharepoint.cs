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
                new SharepointEndpoint { Uri = @"http://sharepoint.com" }
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
            Console.WriteLine($"Attempting to distribute file {file.FileName} to Sharepoint URI {endpoint.Uri}");

            if (new Random().Next(2) == 1) //Generates a random number of 0 or 1.
            {
                throw new Exception($"  - ERROR distributing file { file.FileName } to Sharepoint URI { endpoint.Uri}");
            }
        }

        protected override void OnSuccess(DistributionFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"  - Success distributing file {file.FileName} to Sharepoint URI {endpoint.Uri}");
        }

        protected override void OnError(Exception exception, DistributionFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
        }
    }
}
