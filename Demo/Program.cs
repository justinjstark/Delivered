using System;
using System.Collections.Generic;
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
            var distributor = new Distributor<DistributableFile>();
            distributor.RegisterEndpointRepository(new FileSystemEndpointRepository());
            distributor.RegisterEndpointRepository(new SharepointEndpointRepository());
            distributor.RegisterEndpointDeliveryService(new FileSystemDeliveryService());
            distributor.RegisterEndpointDeliveryService(new SharepointDeliveryService());

            //Distribute all files
            foreach (var distributableFile in GetDistributableFiles())
            {
                distributor.Distribute(distributableFile, "recipient");
            }

            Console.ReadLine();
        }

        private static IEnumerable<DistributableFile> GetDistributableFiles()
        {
            var distributableFiles = new List<DistributableFile>
            {
                new DistributableFile
                {
                    Id = Guid.NewGuid(),
                    RecipientName = "TestRecipient",
                    Name = "test.pdf",
                    Contents = null
                }
            };

            return distributableFiles;
        }
    }
}
