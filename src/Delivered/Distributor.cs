using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delivered
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>, IDisposable
        where TDistributable : IDistributable
        where TRecipient : IRecipient
    {
        private readonly Configuration<TDistributable, TRecipient> _configuration;

        public Distributor(Action<Configuration<TDistributable, TRecipient>> action)
        {
            _configuration = new Configuration<TDistributable, TRecipient>();
            action(_configuration);
        }

        public async Task DistributeAsync(TDistributable distributable, TRecipient recipient)
        {
            var deliveryTasks = new List<Task>();

            foreach (var endpointRepository in _configuration.EndpointRepositories)
            {
                var endpoints = endpointRepository.GetEndpointsForRecipient(recipient);

                foreach (var endpoint in endpoints)
                {
                    IDeliverer deliverer;
                    if (!_configuration.Deliverers.TryGetValue(endpoint.GetType(), out deliverer))
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
            if (_configuration.Semaphore != null)
            {
                await _configuration.Semaphore.WaitAsync().ConfigureAwait(false);
            }

            try
            {
                await deliverer.DeliverAsync(distributable, endpoint).ConfigureAwait(false);
            }
            finally
            {
                _configuration.Semaphore?.Release();
            }
        }

        public void Dispose()
        {
            _configuration.Dispose();
        }
    }
}
