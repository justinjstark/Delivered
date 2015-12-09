using System;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemEndpoint : Endpoint
    {
        public Guid Id { get; set; }
        public string Directory { get; set; }
    }
}
