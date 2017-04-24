using System;
using System.Collections.Generic;
using System.Threading;

namespace Delivered
{
    public class Configuration<TDistributable, TRecipient> : IDisposable
        where TDistributable : IDistributable
        where TRecipient : IRecipient
    {
        internal IList<IEndpointRepository<TRecipient>> EndpointRepositories { get; } = new List<IEndpointRepository<TRecipient>>();

        internal IDictionary<Type, IDeliverer> Deliverers { get; } = new Dictionary<Type, IDeliverer>();

        internal SemaphoreSlim Semaphore { get; private set; }

        public Configuration<TDistributable, TRecipient> RegisterEndpointRepository(IEndpointRepository<TRecipient> endpointRepository)
        {
            if (!EndpointRepositories.Contains(endpointRepository))
            {
                EndpointRepositories.Add(endpointRepository);
            }

            return this;
        }

        public Configuration<TDistributable, TRecipient> RegisterDeliverer<TEndpoint>(IDeliverer<TDistributable, TEndpoint> deliverer)
            where TEndpoint : IEndpoint
        {
            Deliverers[typeof(TEndpoint)] = deliverer;

            return this;
        }

        public Configuration<TDistributable, TRecipient> MaximumConcurrentDeliveries(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            Semaphore = new SemaphoreSlim(number);

            return this;
        }

        protected virtual void Dispose(bool freeManagedObjects)
        {
            if (freeManagedObjects)
            {
                Semaphore?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Configuration()
        {
            Dispose(false);
        }
    }
}