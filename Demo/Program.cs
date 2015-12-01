using System;
using System.Collections.Generic;
using Demo.Endpoints;
using Distributor;

namespace Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var distributables = new List<DistributableFile>
            {
                new DistributableFile
                {
                    Id = Guid.NewGuid(),
                    ProfileName = "TestProfile",
                    Name = "test.pdf",
                    Contents = null
                }
            };

            //Configure the distributor
            var distributor = new Distributor<DistributableFile>();
            distributor.EndpointDeliveryServices.Add(new SharepointDeliveryService());
            distributor.EndpointDeliveryServices.Add(new FileSystemDeliveryService());

            //Run the Distributor
            distributor.Distribute(distributables);

            Console.ReadLine();
        }
    }
}
