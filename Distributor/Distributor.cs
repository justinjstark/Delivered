namespace Distributor
{
    public class Distributor
    {
        public void Run()
        {
            var file = new DistributionFile { FileName = "File.txt" };

            foreach (var deliveryService in DeliveryServices.Distributions)
            {
                deliveryService.DeliverFile(file);
            }
        }
    }
}
