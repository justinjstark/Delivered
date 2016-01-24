using System;
using System.Threading.Tasks;
using DemoDistributor;
using Delivered;

namespace DemoDistributor.Endpoints.Sharepoint
{
    public class SharepointDeliveryService : EndpointDeliveryService<DistributableFile, SharepointEndpoint>
    {
        public SharepointDeliveryService()
        {
            MaximumConcurrentDeliveries(e => new Uri(e.Uri).Host, 1);
            MaximumConcurrentDeliveries(2);
        }

        protected override async Task DoDeliveryAsync(DistributableFile file, SharepointEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to Sharepoint URI {endpoint.Uri}");

            await Task.Delay(1000);

            Console.WriteLine($"Distributed file {file.Name} to Sharepoint URI {endpoint.Uri}");

            await Task.FromResult(0);
        }
    }
}
