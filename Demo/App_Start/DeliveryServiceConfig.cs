using Demo.Endpoints;
using Distributor;

namespace Demo
{
    public static class DeliveryServiceConfig
    {
        public static void RegisterDeliveryServices(EndpointDeliveryServicesCollection endpointDeliveryServices)
        {
            //Add delivery services here.
            endpointDeliveryServices.Add(new SharepointDeliveryService());
            endpointDeliveryServices.Add(new FileSystemDeliveryService());
        }
    }
}
