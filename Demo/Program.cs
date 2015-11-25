﻿using System;
using Distributor;

namespace Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Register the Delivery Service for each Endpoint
            DeliveryServiceConfig.RegisterDeliveryServices(EndpointDeliveryServices.DeliveryServices);

            //Run the Distributor
            var distributor = new Distributor.Distributor();
            distributor.Run();

            Console.ReadLine();
        }
    }
}
