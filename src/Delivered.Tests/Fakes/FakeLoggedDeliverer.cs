using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Delivered.Tests.Fakes
{
    public class FakeLoggedDeliverer<TDistributable, TEndpoint> : Deliverer<TDistributable, TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        public AutoResetEvent DeliveryStarted = new AutoResetEvent(false);

        public class Delivery
        {
            public TDistributable Distributable;
            public TEndpoint Endpoint;
            public AsyncAutoResetEvent Continue = new AsyncAutoResetEvent();
            public bool Complete;
        }

        public Queue<Delivery> Deliveries = new Queue<Delivery>();
        
        public FakeLoggedDeliverer()
        {
        }

        public FakeLoggedDeliverer(IDictionary<Func<TEndpoint, object>, int> groupingFuncs)
        {
            foreach (var keyValuePair in groupingFuncs)
            {
                MaximumConcurrentDeliveries(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint)
        {
            var delivery = new Delivery
            {
                Distributable = distributable,
                Endpoint = endpoint
            };

            Deliveries.Enqueue(delivery);

            DeliveryStarted.Set();

            await delivery.Continue.WaitAsync();

            delivery.Complete = true;
        }

        public Delivery WaitForStart()
        {
            DeliveryStarted.WaitOne();

            return Deliveries.Dequeue();
        }

        public Delivery WaitForStart(TimeSpan timeout)
        {
            var success = DeliveryStarted.WaitOne(timeout);

            if(!success)
                throw new TimeoutException();

            return Deliveries.Dequeue();
        }

        public bool HappensSynchronously()
        {
            
        }

        public bool HappensAsynchronously()
        {

        }
    }
}
