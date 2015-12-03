using System;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemDeliveryService : EndpointDeliveryService<DistributableFile, FileSystemEndpoint>
    {
        public FileSystemDeliveryService() : base(new FileSystemEndpointRepository())
        {
        }

        protected override void DeliverToEndpoint(DistributableFile file, FileSystemEndpoint endpoint)
        {
            Console.WriteLine($"Attempting to distribute file {file.Name} to File System directory {endpoint.Directory}");
        }

        protected override void OnSuccess(DistributableFile file, FileSystemEndpoint endpoint)
        {
            Console.WriteLine($"  - Success distributing file {file.Name} to File System directory {endpoint.Directory}");
        }

        protected override void OnError(DistributableFile file, FileSystemEndpoint endpoint, Exception exception)
        {
            Console.WriteLine($"  - ERROR distributing file {file.Name} to File System directory {endpoint.Directory}");
        }
    }
}
