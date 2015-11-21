using System;
using System.Collections.Generic;
using Distributor;

namespace Demo.Endpoints
{
    public class FileSystemEndpoint : IEndpoint
    {
        public Guid Id { get; set; }
        public string Directory { get; set; }
    }

    public class FileSystemEndpointRepository : IEndpointRepository<FileSystemEndpoint>
    {
        public IEnumerable<FileSystemEndpoint> GetEndpointsForProfile(string profileName)
        {
            return new List<FileSystemEndpoint>
            {
                new FileSystemEndpoint { Directory = @"//path1/path2" },
                new FileSystemEndpoint { Directory = @"//dir1/dir2" }
            };
        }
    }

    public class FileSystemDeliveryService : DeliveryService<FileSystemEndpoint>
    {
        public FileSystemDeliveryService(FileSystemEndpointRepository endpointRepository) : base(endpointRepository)
        {
        }

        protected override void Deliver(Delivery<FileSystemEndpoint> delivery)
        {
            Console.WriteLine($"Attempting to distribute file {delivery.File.FileName} to File System directory {delivery.Endpoint.Directory}");
        }

        protected override void OnSuccess(Delivery<FileSystemEndpoint> delivery)
        {
            Console.WriteLine($"  - Success distributing file {delivery.File.FileName} to File System directory {delivery.Endpoint.Directory}");
        }
        
        protected override void OnError(Delivery<FileSystemEndpoint> delivery, Exception exception)
        {
            Console.WriteLine($"  - ERROR distributing file {delivery.File.FileName} to File System directory {delivery.Endpoint.Directory}");
        }
    }
}
