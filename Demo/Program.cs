using System;
using Demo.Endpoints.FileSystem;
using Demo.Endpoints.Sharepoint;
using Verdeler;

namespace Demo
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
            distributor.MaximumConcurrentDeliveries(10);
            
            //Distribute a file to a vendor
            var distributionTask = distributor.DistributeAsync(FakeFile, FakeVendor);

            Console.WriteLine("All deliveries started.");

            distributionTask.Wait();
            Console.WriteLine("All deliveries complete.");

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
    }
}
