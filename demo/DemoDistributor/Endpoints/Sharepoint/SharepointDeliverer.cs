using System;
using System.Threading.Tasks;
using Delivered;

namespace DemoDistributor.Endpoints.Sharepoint
{
    public class SharepointDeliverer : Deliverer<File, SharepointEndpoint>
    {
        public SharepointDeliverer()
        {
            MaximumConcurrentDeliveries(e => new Uri(e.Uri).Host, 1);
            MaximumConcurrentDeliveries(2);
        }

        public override async Task DeliverAsync(File file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");

            await Task.Delay(1000);

            Console.WriteLine($"Distributed file {file.Name} to Sharepoint URI {endpoint.Uri}");

            await Task.FromResult(0);
        }
    }
}
