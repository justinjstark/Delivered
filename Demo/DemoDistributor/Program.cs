using System;
using DemoDistributor.Endpoints.FileSystem;
using DemoDistributor.Endpoints.Sharepoint;
using Delivered;

namespace DemoDistributor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Configure the distributor
            var distributor = new Distributor<DistributableFile, Vendor>();
            distributor.RegisterEndpointRepository(new FileSystemEndpointRepository());
            distributor.RegisterEndpointRepository(new SharepointEndpointRepository());
            distributor.RegisterEndpointDeliveryService(new FileSystemDeliveryService());
            distributor.RegisterEndpointDeliveryService(new SharepointDeliveryService());
            distributor.MaximumConcurrentDeliveries(3);
            
            //Distribute a file to a vendor
            try
            {
                var task = distributor.DistributeAsync(FakeFile, FakeVendor);
                task.Wait();

                Console.WriteLine("\nDistribution succeeded.");
            }
            catch(AggregateException aggregateException)
            {
                Console.WriteLine("\nDistribution failed.");

                foreach (var exception in aggregateException.Flatten().InnerExceptions)
                {
                    Console.WriteLine($"* {exception.Message}");
                }
            }

            Console.WriteLine("\nPress enter to exit.");

            Console.ReadLine();
        }

        private static DistributableFile FakeFile => new DistributableFile
        {
            Name = @"test.pdf",
            Contents = new byte[1024]
        };

        private static Vendor FakeVendor => new Vendor
        {
            Name = @"Mark's Pool Supplies"
        };

        private static Vendor FakeVendor2 => new Vendor
        {
            Name = @"Kevin's Art Supplies"
        };
    }
}
