using Demo.Endpoints;
using Distributor;

namespace Demo
{
    public static class DeliveryServiceConfig
    {
        public static void RegisterDeliveryServices(DeliveryServicesCollection deliveryServices)
        {
            //Add delivery services here.
            deliveryServices.Add(new SharepointDeliveryService(new SharepointEndpointRepository()));
            deliveryServices.Add(new FileSystemDeliveryService(new FileSystemEndpointRepository()));
        }
    }
}
