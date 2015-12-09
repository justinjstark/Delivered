using System.Collections.Generic;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemEndpointRepository : IEndpointRepository<Vendor>
    {
        public IEnumerable<IEndpoint> GetEndpointsForRecipient(Vendor vendor)
        {
            return new List<FileSystemEndpoint>
            {
                new FileSystemEndpoint { Directory = @"//path1/path2" },
                new FileSystemEndpoint { Directory = @"//dir1/dir2" }
            };
        }
    }
}
