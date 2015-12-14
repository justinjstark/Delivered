using System;
using System.Threading;
using System.Threading.Tasks;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemDeliveryService : EndpointDeliveryService<DistributableFile, FileSystemEndpoint>
    {
        public override Task DeliverAsync(DistributableFile file, FileSystemEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to File System directory {endpoint.Directory}");

            Thread.Sleep(2000);

            Console.WriteLine($"Distributed file {file.Name} to File System directory {endpoint.Directory}");

            return Task.FromResult(true);
        }
    }
}
