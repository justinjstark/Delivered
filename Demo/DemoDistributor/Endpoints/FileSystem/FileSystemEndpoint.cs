using System;
using Delivered;

namespace DemoDistributor.Endpoints.FileSystem
{
    public class FileSystemEndpoint : Endpoint
    {
        public Guid Id { get; set; }
        public string Directory { get; set; }
    }
}
