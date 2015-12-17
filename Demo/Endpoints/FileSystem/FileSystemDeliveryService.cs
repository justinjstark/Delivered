﻿using System;
using System.Threading.Tasks;
using Verdeler;

namespace Demo.Endpoints.FileSystem
{
    public class FileSystemDeliveryService : EndpointDeliveryService<DistributableFile, FileSystemEndpoint>
    {
        public FileSystemDeliveryService()
        {
            MaximumConcurrentDeliveries(10);
            MaximumConcurrentDeliveriesPerRecipient(1);
        }

        protected override async Task DoDeliveryAsync(DistributableFile file, FileSystemEndpoint endpoint)
        {
            Console.WriteLine($"Distributing file {file.Name} to File System directory {endpoint.Directory}");

            await Task.Delay(2000);

            throw new Exception("Oh crap!");

            Console.WriteLine($"Distributed file {file.Name} to File System directory {endpoint.Directory}");

            await Task.FromResult(0);
        }
    }
}
