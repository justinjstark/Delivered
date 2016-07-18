using System;
using System.Collections.Generic;
using Delivered.Tests.Helpers.Fakes;
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

            areAsynchronous.ShouldBeTrue();
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

            areSynchronous.ShouldBeTrue();
        }

        [Test]
        public void DeliverAsync_DeliversOneAtATimeWhenLimitedToOneByGroup()
        {
            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "1" };

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

            var controller = new Controller<FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>>(deliverer);

            var areSynchronous = controller.HappenSynchronously(
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint1),
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint2));

            areSynchronous.ShouldBeTrue();
        }

        [Test]
        public void DeliverAsync_DoesNotThrottleWhenGroupedToNull()
        {
            var endpoint1 = new FakeEndpoint();
            var endpoint2 = new FakeEndpoint();

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => null, 1 } });

            var controller = new Controller<FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>>(deliverer);

            var areSynchronous = controller.HappenAsynchronously(
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint1),
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint2));

            areSynchronous.ShouldBeTrue();
        }

        [Test]
        public void DeliverAsync_DeliversAtSameTimeWhenEndpointsAreGroupedDifferently()
        {
            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "2" };

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

            var controller = new Controller<FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>>(deliverer);

            var areSynchronous = controller.HappenAsynchronously(
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint1),
                async () => await ((IDeliverer)deliverer).DeliverAsync(new FakeDistributable(), endpoint2));

            areSynchronous.ShouldBeTrue();
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
