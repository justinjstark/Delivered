using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Verdeler
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        private SemaphoreSlim _semaphore;

        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

        public void MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            _semaphore = new SemaphoreSlim(number);
        }

        public void RegisterEndpointRepository(IEndpointRepository<TRecipient> endpointRepository)
        {
            if (!_endpointRepositories.Contains(endpointRepository))
            {
                _endpointRepositories.Add(endpointRepository);
            }
        }

        public void RegisterEndpointDeliveryService<TEndpoint>(IEndpointDeliveryService<TEndpoint> endpointDeliveryService)
            where TEndpoint : Endpoint
        {
            _endpointDeliveryServices[typeof(TEndpoint)] = endpointDeliveryService;
        }

        public async Task DistributeAsync(TDistributable distributable, TRecipient recipient)
        {
            var deliveryTasks = new List<Task>();

            foreach (var endpointRepository in _endpointRepositories)
            {
                var endpoints = endpointRepository.GetEndpointsForRecipient(recipient);

                foreach (var endpoint in endpoints)
                {
                    var endpointDeliveryService = _endpointDeliveryServices[endpoint.GetType()];

                    var deliveryTask = DeliverAsync(endpointDeliveryService, distributable, endpoint, recipient);
                    deliveryTasks.Add(deliveryTask);
                }
            }

            //Wait for all deliveries to complete
            var task = Task.WhenAll(deliveryTasks);

            //This try-catch forces all deliveries to finish before an aggregate exception is thrown.
            try
            {
                await task;
            }
            catch
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
                throw;
            }
        }

        private async Task DeliverAsync(IEndpointDeliveryService endpointDeliveryService,
            TDistributable distributable, Endpoint endpoint, Recipient recipient)
        {
            if (_semaphore != null)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
            }

            try
            {
                await endpointDeliveryService.DeliverAsync(distributable, endpoint, recipient).ConfigureAwait(false);
            }
            finally
            {
                _semaphore?.Release();
            }
        }
    }
}
