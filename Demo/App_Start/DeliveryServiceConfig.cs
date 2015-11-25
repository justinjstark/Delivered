using Demo.Endpoints;
using Distributor;

namespace Demo
{
    public static class DeliveryServiceConfig
    {
        public static void RegisterDeliveryServices(EndpointDeliveryServicesCollection endpointDeliveryServices)
        {
            //Add delivery services here.
            endpointDeliveryServices.Add(new SharepointDeliveryService(new SharepointEndpointRepository()));
            endpointDeliveryServices.Add(new FileSystemDeliveryService(new FileSystemEndpointRepository()));
        }
    }
}
