using System;

namespace Distributor
{
    class Program
    {
        static void Main(string[] args)
        {
            DeliveryServiceConfig.RegisterDistributionMethods(DeliveryServices.Distributions);

            //Run
            var file = new File {Name = "File.txt"};

            foreach (var deliveryService in DeliveryServices.Distributions)
            {
                deliveryService.DeliverFile(file);
            }

            Console.ReadLine();
        }
    }
}
