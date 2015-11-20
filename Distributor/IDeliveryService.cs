using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
namespace Distributor
{
    public interface IDeliveryService
    {
        void DeliverFile(File file);
    }

    public abstract class DeliveryService<TEndpoint> : IDeliveryService where TEndpoint : IEndpoint
    {
        protected abstract void DeliverFileToEndpoint(File file, TEndpoint endpoint);

        private readonly IEndpointRepository<TEndpoint> _endpointRepository;

        protected DeliveryService(IEndpointRepository<TEndpoint> endpointRepository)
        {
            _endpointRepository = endpointRepository;
        }

        public void DeliverFile(File file)
        {
            var endpoints = _endpointRepository.GetEndpointsForProfile(file.ProfileName);

            foreach (var endpoint in endpoints)
            {
                DeliverFileToEndpoint(file, endpoint);
            }
        }
    }
}
