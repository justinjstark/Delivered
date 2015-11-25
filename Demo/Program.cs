using System;
using System.Collections.Generic;
using Distributor;

namespace Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Register the Delivery Service for each Endpoint
            DeliveryServiceConfig.RegisterDeliveryServices(EndpointDeliveryServices.DeliveryServices);

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

            //Run the Distributor
            var distributor = new Distributor<DistributableFile>();
            distributor.Distribute(distributables);

            Console.ReadLine();
        }
    }
}
