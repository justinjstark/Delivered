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
            distributor.EndpointDeliveryServices.Add(new SharepointDeliveryService());
            distributor.EndpointDeliveryServices.Add(new FileSystemDeliveryService());

            //Distribute all files
            foreach (var distributableFile in GetDistributableFiles())
            {
                distributor.Distribute(distributableFile);
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
