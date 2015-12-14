using System;
using System.Threading;
using System.Threading.Tasks;
using Verdeler;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointDeliveryService : EndpointDeliveryService<DistributableFile, SharepointEndpoint>
    {
        public override Task DeliverAsync(DistributableFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");

            Thread.Sleep(1000);

            Console.WriteLine($"Distributed file {file.Name} to Sharepoint URI {endpoint.Uri}");

            return Task.FromResult(true);
        }
    }
}
