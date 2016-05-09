using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delivered.Tests.Fakes;
using NUnit.Framework;
using Shouldly;

namespace Delivered.Tests
{
    [TestFixture]
    public class DelivererTests
    {
        [Test]
        public void DeliverAsync_DeliversMultipleAtATimeByDefault()
        {
            var endpoint = new FakeEndpoint();

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));

            var task1 = ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint);
            var task2 = ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint);
        
            Task.WaitAll(task1, task2);

            var lastStartTime = deliverer.LogEntries.Max(e => e.StartDateTime);
            var firstEndTime = deliverer.LogEntries.Min(e => e.EndDateTime);

            lastStartTime.ShouldBeLessThan(firstEndTime);
        }

        [Test]
        public void DeliverAsync_DeliversOneAtATimeWithConcurrencyLimitOne()
        {
            var distributable1 = new FakeDistributable();
            var distributable2 = new FakeDistributable();
            var endpoint = new FakeEndpoint();
            
            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));
            deliverer.MaximumConcurrentDeliveries(1);

            var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable1, endpoint);
            var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable2, endpoint);

            Task.WaitAll(task1, task2);

            var task1EndTime = deliverer.LogEntries.Single(e => e.Distributable == distributable1).EndDateTime;
            var task2StartTime = deliverer.LogEntries.Single(e => e.Distributable == distributable2).StartDateTime;

            task1EndTime.ShouldBeLessThanOrEqualTo(task2StartTime);
        }

        [Test]
        public void DeliverAsync_DeliversOneAtATimeWhenLimitedToOneByGroup()
        {
            var distributable = new FakeDistributable();
            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "1" };

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100),
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

            var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);
            var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);

            Task.WaitAll(task1, task2);

            var task1EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
            var task2StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;

            task1EndTime.ShouldBeLessThanOrEqualTo(task2StartTime);
        }

        [Test]
        public void DeliverAsync_DeliversAtSameTimeWhenEndpointsAreGroupedDifferently()
        {
            var distributable = new FakeDistributable();
            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "2" };

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 200),
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

            var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);
            var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);

            Task.WaitAll(task1, task2);

            var task1EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
            var task2StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;

            task2StartTime.ShouldBeLessThan(task1EndTime);
        }

        [Test]
        public void DeliverAsync_ThrottlesExceptWhenGroupedToNull()
        {
            var distributable = new FakeDistributable();
            var endpoint1 = new FakeEndpoint { Port = 0 };
            var endpoint2 = new FakeEndpoint { Port = 1 };
            var endpoint3 = new FakeEndpoint { Port = 1 };

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100),
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Port == 1 ? (int?)null : 1, 1 } });

            var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);
            var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);
            var task3 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint3);

            Task.WaitAll(task1, task2, task3);

            var task1EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
            var task2StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;
            var task2EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).EndDateTime;
            var task3StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint3).StartDateTime;

            task2StartTime.ShouldBeLessThan(task1EndTime);
            task3StartTime.ShouldBeLessThan(task1EndTime);
            task3StartTime.ShouldBeLessThan(task2EndTime);
        }

        [Test]
        public void DeliverAsync_DoesNotThrottleWhenMultipleGroupedToNull()
        {
            var distributable = new FakeDistributable();
            var endpoint1 = new FakeEndpoint();
            var endpoint2 = new FakeEndpoint();

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100),
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => null, 1 } });

            var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);
            var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);

            Task.WaitAll(task1, task2);

            var task1EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
            var task2StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;

            task2StartTime.ShouldBeLessThan(task1EndTime);
        }

        #region "Fakes"

        public class FakeDistributable : IDistributable
        {
        }

        public class FakeEndpoint : IEndpoint
        {
            public string Host { get; set; }
            public int Port { get; set; }
        }

        #endregion "Fakes"
    }
}
