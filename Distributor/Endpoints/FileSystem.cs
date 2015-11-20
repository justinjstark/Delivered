using System;
using System.Collections.Generic;

namespace Distributor.Endpoints
{
    public class FileSystemEndpoint : IEndpoint
    {
        public string Directory { get; set; }
    }

    public class FileSystemEndpointRepository : IEndpointRepository<FileSystemEndpoint>
    {
        public IEnumerable<FileSystemEndpoint> GetEndpointsForProfile(string profileName)
        {
            return new List<FileSystemEndpoint>
            {
                new FileSystemEndpoint { Directory = "site1" },
                new FileSystemEndpoint { Directory = "site2" }
            };
        }
    }

    public class FileSystemDeliveryService : DeliveryService<FileSystemEndpoint>
    {
        public FileSystemDeliveryService(FileSystemEndpointRepository endpointRepository) : base(endpointRepository)
        {
        }

        protected override void DeliverFileToEndpoint(File file, FileSystemEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to File System directory {endpoint.Directory}");
        }
    }
}
