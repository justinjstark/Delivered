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

        protected override void Deliver(Delivery<SharepointEndpoint> delivery)
        {
            Console.WriteLine($"Attempting to distribute file {delivery.File.FileName} to Sharepoint URI {delivery.Endpoint.Uri}");

            if (new Random().Next(2) == 1) //Generates a random number of 0 or 1.
            {
                throw new Exception($"  - ERROR distributing file { delivery.File.FileName } to Sharepoint URI { delivery.Endpoint.Uri}");
            }
        }

        protected override void OnSuccess(Delivery<SharepointEndpoint> delivery)
        {
            Console.WriteLine($"  - Success distributing file {delivery.File.FileName} to Sharepoint URI {delivery.Endpoint.Uri}");
        }

        protected override void OnError(Delivery<SharepointEndpoint> delivery, Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
        }
    }
}
