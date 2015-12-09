using System;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemDeliveryService : EndpointDeliveryService<DistributableFile, FileSystemEndpoint>
    {
        public override void Deliver(DistributableFile file, FileSystemEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to File System directory {endpoint.Directory}");
        }
    }
}
