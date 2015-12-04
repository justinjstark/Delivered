using System;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemDeliveryService : IEndpointDeliveryService<DistributableFile, FileSystemEndpoint>
    {
        public void Deliver(DistributableFile file, FileSystemEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to File System directory {endpoint.Directory}");
        }
    }
}
