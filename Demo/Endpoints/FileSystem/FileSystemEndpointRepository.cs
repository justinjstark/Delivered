using System.Collections.Generic;
using Distributor;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemEndpointRepository : IEndpointRepository<FileSystemEndpoint>
    {
        public IEnumerable<FileSystemEndpoint> GetEndpointsForRecipient(string recipientName)
        {
            return new List<FileSystemEndpoint>
            {
                new FileSystemEndpoint { Directory = @"//path1/path2" },
                new FileSystemEndpoint { Directory = @"//dir1/dir2" }
            };
        }
    }
}
