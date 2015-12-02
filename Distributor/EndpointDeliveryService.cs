using System;

namespace Distributor
{
    public abstract class EndpointDeliveryService<TDistributable, TEndpoint> : IEndpointDeliveryService
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        private readonly IEndpointRepository<TEndpoint> _endpointRepository;
        private IDeliveryRepository _deliveryRepository;
        
        protected EndpointDeliveryService(IEndpointRepository<TEndpoint> endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        protected abstract void DeliverToEndpoint(TDistributable distributable, TEndpoint endpoint);

        public void UseDeliveryRepository(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }
        
        public void Deliver(IDistributable distributable)
        {
            var endpoints = _endpointRepository.GetEndpointsForRecipient(distributable.RecipientName);

            foreach (var endpoint in endpoints)
            {
                try
                {
                    DeliverToEndpoint((TDistributable) distributable, endpoint);

                    _deliveryRepository?.RecordDelivery(new Delivery(distributable.Id, endpoint.Id));

                    OnSuccess((TDistributable) distributable, endpoint);

                    //TODO: Mark file as delivered to endpoint.
                    //Not all deliveries may be idempotent so we only ever want to deliver a file once.
                }
                catch (Exception exception)
                {
                    //Call virtual OnError method and then continue to the next endpoint delivery.
                    OnError((TDistributable) distributable, endpoint, exception);
                }
            }
        }

        protected virtual void OnSuccess(TDistributable distributable, TEndpoint endpoint)
        {
        }

        protected virtual void OnError(TDistributable distributable, TEndpoint endpoint, Exception exception)
        {
        }
    }
}
