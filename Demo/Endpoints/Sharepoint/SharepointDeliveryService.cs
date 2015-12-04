using System;
using Verdeler;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointDeliveryService : IEndpointDeliveryService<DistributableFile, SharepointEndpoint>
    {
        public void Deliver(DistributableFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");
        }
    }
}
