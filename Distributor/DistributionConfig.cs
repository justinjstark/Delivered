using Distributor.Endpoints;

namespace Distributor
{
    public static class DistributionConfig
    {
        public static void RegisterDistributionMethods(DistributionMethodsCollection distributionMethods)
        {
            //Add any distribution methods here.
            distributionMethods.Add(new DistributionMethod(new SharepointEndpointRepository(), new SharepointDeliveryService()));
            distributionMethods.Add(new DistributionMethod(new FileSystemEndpointRepository(), new FileSystemDeliveryService()));
        }
    }
}
