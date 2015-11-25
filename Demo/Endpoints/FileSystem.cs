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

    public class FileSystemDeliveryService : EndpointDeliveryService<DistributableFile, FileSystemEndpoint>
    {
        public FileSystemDeliveryService(FileSystemEndpointRepository endpointRepository) : base(endpointRepository)
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
