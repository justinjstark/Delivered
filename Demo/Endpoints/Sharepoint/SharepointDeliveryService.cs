using System;
using Verdeler;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointDeliveryService : EndpointDeliveryService<DistributableFile, SharepointEndpoint>
    {
        public SharepointDeliveryService() : base(new SharepointEndpointRepository())
        {
        }

        protected override void DeliverToEndpoint(DistributableFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Attempting to distribute file {file.Name} to Sharepoint URI {endpoint.Uri}");

            if (new Random().Next(2) == 1) //Generates a random number of 0 or 1.
            {
                throw new Exception($"  - ERROR distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");
            }
        }

        protected override void OnSuccess(DistributableFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"  - Success distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");
        }

        protected override void OnError(DistributableFile file, SharepointEndpoint endpoint, Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
        }
    }
}
