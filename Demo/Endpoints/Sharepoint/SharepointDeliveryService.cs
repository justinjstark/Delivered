using System;
using Verdeler;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointDeliveryService : EndpointDeliveryService<DistributableFile, SharepointEndpoint>
    {
        public override void Deliver(DistributableFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");
        }
    }
}
