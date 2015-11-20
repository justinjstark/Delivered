using System;

namespace Distributor
{
    class Program
    {
        static void Main(string[] args)
        {
            DistributionConfig.RegisterDistributionMethods(DistributionMethods.Distributions);

            //Run
            var file = new File {Name = "File.txt"};

            foreach (var distributor in DistributionMethods.Distributions)
            {
                distributor.DeliverToEndpoints(file, "profileName");
            }

            Console.ReadLine();
        }
    }
}
