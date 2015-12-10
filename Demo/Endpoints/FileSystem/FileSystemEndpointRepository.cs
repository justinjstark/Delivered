using System;
using System.Collections.Generic;
using System.Threading;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemEndpointRepository : IEndpointRepository<Vendor>
    {
        public IEnumerable<Endpoint> GetEndpointsForRecipient(Vendor vendor)
        {
            Console.WriteLine("Getting file system endpoints");

            Thread.Sleep(1000);

            Console.WriteLine("Got file system endpoints");

            return new List<FileSystemEndpoint>
            {
                new FileSystemEndpoint { Directory = @"//path1/path2" },
                new FileSystemEndpoint { Directory = @"//dir1/dir2" }
            };
        }
    }
}
