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

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>();

            var controller = new Controller<FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>>(deliverer);

            var areAsynchronous = controller.HappenAsynchronously(
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint),
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint));

            areAsynchronous.ShouldBe(true);
        }

        [Test]
        public void DeliverAsync_DeliversOneAtATimeWithConcurrencyLimitOne()
        {
            var endpoint = new FakeEndpoint();

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>();
            deliverer.MaximumConcurrentDeliveries(2);

            var controller = new Controller<FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>>(deliverer);

            var areSynchronous = controller.HappenSynchronously(
                async () => await ((IDeliverer) deliverer).DeliverAsync(new FakeDistributable(), endpoint),
                async () => await ((IDeliverer) deliverer).DeliverAsync(new FakeDistributable(), endpoint));

            areSynchronous.ShouldBe(true);
        }

        //[Test]
        //public void DeliverAsync_DeliversOneAtATimeWhenLimitedToOneByGroup()
        //{
        //    var distributable = new FakeDistributable();
        //    var endpoint1 = new FakeEndpoint { Host = "1" };
        //    var endpoint2 = new FakeEndpoint { Host = "1" };

        //    var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(
        //        new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

        //    ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);

        //    //Wait for 1 to start before executing 2 to prevent a race condition
        //    deliverer.WaitForStart();

        //    ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);

        //    //Wait 1 second for task2 to start
        //    Should.Throw<TimeoutException>(() => deliverer.WaitForStart(new TimeSpan(0, 0, 0, 0, 200)));
        //}

        //[Test]
        //public void DeliverAsync_DeliversAtSameTimeWhenEndpointsAreGroupedDifferently()
        //{
        //    var distributable = new FakeDistributable();
        //    var endpoint1 = new FakeEndpoint { Host = "1" };
        //    var endpoint2 = new FakeEndpoint { Host = "2" };

        //    var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(
        //        new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

        //    var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);
        //    var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);

        //    Task.WaitAll(task1, task2);

        //    var task1EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
        //    var task2StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;

        //    task2StartTime.ShouldBeLessThan(task1EndTime);
        //}

        //[Test]
        //public void DeliverAsync_ThrottlesExceptWhenGroupedToNull()
        //{
        //    var distributable = new FakeDistributable();
        //    var endpoint1 = new FakeEndpoint { Port = 0 };
        //    var endpoint2 = new FakeEndpoint { Port = 1 };
        //    var endpoint3 = new FakeEndpoint { Port = 1 };

        //    var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(
        //        new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Port == 1 ? (int?)null : 1, 1 } });

        //    var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);
        //    var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);
        //    var task3 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint3);

        //    Task.WaitAll(task1, task2, task3);

        //    var task1EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
        //    var task2StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;
        //    var task2EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).EndDateTime;
        //    var task3StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint3).StartDateTime;

        //    task2StartTime.ShouldBeLessThan(task1EndTime);
        //    task3StartTime.ShouldBeLessThan(task1EndTime);
        //    task3StartTime.ShouldBeLessThan(task2EndTime);
        //}

        //[Test]
        //public void DeliverAsync_DoesNotThrottleWhenMultipleGroupedToNull()
        //{
        //    var distributable = new FakeDistributable();
        //    var endpoint1 = new FakeEndpoint();
        //    var endpoint2 = new FakeEndpoint();

        //    var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(
        //        new Dictionary<Func<FakeEndpoint, object>, int> { { e => null, 1 } });

        //    var task1 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint1);
        //    var task2 = ((IDeliverer)deliverer).DeliverAsync(distributable, endpoint2);

        //    Task.WaitAll(task1, task2);

        //    var task1EndTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
        //    var task2StartTime = deliverer.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;

        //    task2StartTime.ShouldBeLessThan(task1EndTime);
        //}

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
