using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Verdeler
{
    public class Distributor<TDistributable, TRecipient> : IDistributor<TDistributable, TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        public Synchrony EndpointRetrievalSynchrony;
        public Synchrony DeliverySynchrony;

        private readonly List<IEndpointRepository<TRecipient>> _endpointRepositories
            = new List<IEndpointRepository<TRecipient>>();

        private readonly Dictionary<Type, IEndpointDeliveryService> _endpointDeliveryServices
            = new Dictionary<Type, IEndpointDeliveryService>();

        public Distributor()
        {
            EndpointRetrievalSynchrony = Synchrony.Asynchronous;
            DeliverySynchrony = Synchrony.Asynchronous;
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

        public Task Distribute(TDistributable distributable, TRecipient recipient)
        {
            var deliveryTasks = new List<Task>();

            var getEndpointsTaskScheduler = new LimitedConcurrencyLevelTaskScheduler(EndpointRetrievalSynchrony == Synchrony.Synchronous ? 1 : 99);
            var getEndpointsTaskFactory = new TaskFactory(getEndpointsTaskScheduler);

            var deliverToEndpointsTaskScheduler = new LimitedConcurrencyLevelTaskScheduler(DeliverySynchrony == Synchrony.Synchronous ? 1 : 99);
            var deliverToEndpointsTaskFactory = new TaskFactory(deliverToEndpointsTaskScheduler);

            foreach (var endpointRepository in _endpointRepositories)
            {
                var getEndpointsTask = getEndpointsTaskFactory.StartNew(() => endpointRepository.GetEndpointsForRecipient(recipient));

                var deliverToEndpointsTasks = DeliverToEndpoints(deliverToEndpointsTaskFactory, getEndpointsTask.Result, distributable);

                deliveryTasks.AddRange(deliverToEndpointsTasks);
            }

            return Task.WhenAll(deliveryTasks);
        }

        private IEnumerable<Task> DeliverToEndpoints(TaskFactory deliverToEndpointsTaskFactory, IEnumerable<Endpoint> endpoints, TDistributable distributable)
        {
            var deliveryTasks = new List<Task>();

            foreach (var endpoint in endpoints)
            {
                var endpointDeliveryService = _endpointDeliveryServices[endpoint.GetType()];

                var deliveryTask = deliverToEndpointsTaskFactory.StartNew(() => endpointDeliveryService.Deliver(distributable, endpoint), TaskCreationOptions.AttachedToParent);

                deliveryTasks.Add(deliveryTask);
            }

            return deliveryTasks;
        }
    }
}
