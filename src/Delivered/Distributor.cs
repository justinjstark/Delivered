using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Delivered
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>, IDisposable
        where TDistributable : IDistributable
        where TRecipient : IRecipient
    {
        private SemaphoreSlim _semaphore;

        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, IDeliverer> _deliverers
            = new Dictionary<Type, IDeliverer>();

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

        public void RegisterDeliverer<TEndpoint>(IDeliverer<TDistributable, TEndpoint> deliverer)
            where TEndpoint : IEndpoint
        {
            _deliverers[typeof(TEndpoint)] = deliverer;
        }

        public async Task DistributeAsync(TDistributable distributable, TRecipient recipient)
        {
            var deliveryTasks = new List<Task>();

            foreach (var endpointRepository in _endpointRepositories)
            {
                var endpoints = endpointRepository.GetEndpointsForRecipient(recipient);

                foreach (var endpoint in endpoints)
                {
                    IDeliverer deliverer;
                    if (!_deliverers.TryGetValue(endpoint.GetType(), out deliverer))
                    {
                        throw new InvalidOperationException(
                            $"No endpoint delivery service registered for endpoint type {endpoint.GetType()}");
                    }

                    var deliveryTask = DeliverAsync(deliverer, distributable, endpoint);
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

        private async Task DeliverAsync(IDeliverer deliverer,
            TDistributable distributable, IEndpoint endpoint)
        {
            if (_semaphore != null)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
            }

            try
            {
                await deliverer.DeliverAsync(distributable, endpoint).ConfigureAwait(false);
            }
            finally
            {
                _semaphore?.Release();
            }
        }

        protected virtual void Dispose(bool itIsSafeToAlsoFreeManagedObjects)
        {
            if (_semaphore != null)
            {
                _semaphore.Dispose();
                _semaphore = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Distributor()
        {
            Dispose(false);
        }
    }
}
