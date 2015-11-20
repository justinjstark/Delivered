using Distributor.Endpoints;

namespace Distributor
{
    public static class DeliveryServiceConfig
    {
        public static void RegisterDistributionMethods(DeliveryServicesCollection distributionMethods)
        {
            //Add any distribution methods here.
            distributionMethods.Add(new SharepointDeliveryService(new SharepointEndpointRepository()));
            distributionMethods.Add(new FileSystemDeliveryService(new FileSystemEndpointRepository()));
        }
    }
}
