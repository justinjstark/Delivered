using System;
using System.Collections.Generic;

namespace Distributor.Endpoints
{
    public class FileSystemEndpoint : IEndpoint
    {
        public string Directory { get; set; }
    }

    public class FileSystemEndpointRepository : IEndpointRepository
    {
        public IEnumerable<IEndpoint> GetEndpointsForProfile(string profileName)
        {
            return new List<IEndpoint>
            {
                new FileSystemEndpoint { Directory = "site1" },
                new FileSystemEndpoint { Directory = "site2" }
            };
        }
    }

    public class FileSystemDeliveryService : IDeliveryService
    {
        public void DeliverToEndpoints(File file, IEndpoint endpoint)
        {
            var fileSystemEndpoint = (FileSystemEndpoint) endpoint;

            Console.WriteLine($"Delivering file {file.Name} to File System directory {fileSystemEndpoint.Directory}");
        }
    }
}
