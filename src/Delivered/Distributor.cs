using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Delivered
{
    public class DistributorConfiguration<TDistributable, TRecipient>
        where TDistributable : IDistributable
        where TRecipient : IRecipient
    {
        internal int? MaximumConcurrentDeliveries;

        private readonly Distributor<TDistributable, TRecipient> _distributor;

        public DistributorConfiguration(Distributor<TDistributable, TRecipient> distributor)
        {
            _distributor = distributor;
        }

        public void SetMaximumConcurrentDeliveries(int maximumConcurrentDeliveries)
        {
            if (maximumConcurrentDeliveries <= 0)
            {
                throw new ArgumentException("Maximum concurrent deliveries must be greater than zero.", nameof(maximumConcurrentDeliveries));
            }

            MaximumConcurrentDeliveries = maximumConcurrentDeliveries;
        }

        public void AddEndpointRepository(IEndpointRepository<TRecipient> endpointRepository)
        {
            if (!_distributor.EndpointRepositories.Contains(endpointRepository))
            {
                _distributor.EndpointRepositories.Add(endpointRepository);
            }
        }

        public void AddDeliverer<TEndpoint>(IDeliverer<TDistributable, TEndpoint> deliverer)
            where TEndpoint : IEndpoint
        {
            _distributor.Deliverers[typeof(TEndpoint)] = deliverer;
        }
    }

    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>
        where TDistributable : IDistributable
        where TRecipient : IRecipient
    {
        private static DistributorConfiguration<TDistributable, TRecipient> _configuration;
        
        internal readonly List<IEndpointRepository<TRecipient>> EndpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        internal readonly Dictionary<Type, IDeliverer> Deliverers
            = new Dictionary<Type, IDeliverer>();

        public Distributor(Action<DistributorConfiguration<TDistributable, TRecipient>> configure)
        {
            _configuration = new DistributorConfiguration<TDistributable, TRecipient>(this);
            configure(_configuration);
        }

        internal void RegisterEndpointRepository(IEndpointRepository<TRecipient> endpointRepository)
        {
            if (!EndpointRepositories.Contains(endpointRepository))
            {
                EndpointRepositories.Add(endpointRepository);
            }
        }

        internal void RegisterDeliverer<TEndpoint>(IDeliverer<TDistributable, TEndpoint> deliverer)
            where TEndpoint : IEndpoint
        {
            Deliverers[typeof(TEndpoint)] = deliverer;
        }

        public async Task DistributeAsync(TDistributable distributable, TRecipient recipient)
        {
            var deliveryTasks = new List<Task>();

            foreach (var endpointRepository in EndpointRepositories)
            {
                var endpoints = endpointRepository.GetEndpointsForRecipient(recipient);

                foreach (var endpoint in endpoints)
                {
                    IDeliverer deliverer;
                    if (!Deliverers.TryGetValue(endpoint.GetType(), out deliverer))
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

        private static async Task DeliverAsync(IDeliverer deliverer,
            TDistributable distributable, IEndpoint endpoint)
        {
            var semaphore = _configuration.MaximumConcurrentDeliveries == null ? null :
                new SemaphoreSlim(_configuration.MaximumConcurrentDeliveries.Value);

            if (semaphore != null)
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
            }

            try
            {
                await deliverer.DeliverAsync(distributable, endpoint).ConfigureAwait(false);
            }
            finally
            {
                semaphore?.Release();
            }
        }
    }
}
