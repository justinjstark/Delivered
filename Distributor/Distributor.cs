namespace Distributor
{
    public class Distributor
    {
        public void Run()
        {
            var file = new DistributionFile { FileName = "File.txt" };

            foreach (var endpointDeliveryService in EndpointDeliveryServices.DeliveryServices)
            {
                endpointDeliveryService.DeliverFile(file);
            }
        }
    }
}
