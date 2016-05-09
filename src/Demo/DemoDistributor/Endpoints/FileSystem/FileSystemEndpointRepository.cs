using System;
using System.Collections.Generic;
using System.Threading;
using Delivered;

namespace DemoDistributor.Endpoints.FileSystem
{
    public class FileSystemEndpointRepository
    {
        public IEnumerable<IEndpoint> GetEndpointsForRecipient(Vendor vendor)
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
