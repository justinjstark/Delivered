using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delivered.Tests.Fakes;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Delivered.Tests
{
    [TestFixture]
    public class DistributorTests
    {
        [Test]
        public void DistributeAsync_DeliversToAnEndpointTypeWithNoDelivererThrowsAnError()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            Func<FakeRecipient, IEnumerable<FakeEndpoint>> endpointGetter = r => new[] { endpoint };

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.GetEndpointsUsing(endpointGetter);

            Should.ThrowAsync<InvalidOperationException>(distributor.DistributeAsync(distributable, recipient));
        }

        [Test]
        public void DistributeAsync_DeliversADistributableToARegisteredDeliverer()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var deliverer = new Mock<IDeliverer<FakeDistributable, FakeEndpoint>>();

            Func<FakeRecipient, IEnumerable<FakeEndpoint>> endpointGetter = r => new[] { endpoint };

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.GetEndpointsUsing(endpointGetter);
            distributor.DeliverUsing(deliverer.Object);
            
            distributor.DistributeAsync(distributable, recipient).Wait();

            deliverer.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint) endpoint), Times.Once);
        }

        [Test]
        public void DistributeAsync_DoesNotUseTheFirstDelivererWhenTwoAreRegistered()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var deliverer1 = new Mock<IDeliverer<FakeDistributable, FakeEndpoint>>();
            var deliverer2 = new Mock<IDeliverer<FakeDistributable, FakeEndpoint>>();

            Func<FakeRecipient, IEnumerable<FakeEndpoint>> endpointGetter = r => new[] { endpoint };

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.GetEndpointsUsing(endpointGetter);
            distributor.DeliverUsing(deliverer1.Object);
            distributor.DeliverUsing(deliverer2.Object);

            distributor.DistributeAsync(distributable, recipient).Wait();

            deliverer1.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint) endpoint), Times.Never);
        }

        [Test]
        public void DistributeAsync_UsesTheSecondDelivererWhenTwoAreRegistered()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var deliverer1 = new Mock<IDeliverer<FakeDistributable, FakeEndpoint>>();
            var deliverer2 = new Mock<IDeliverer<FakeDistributable, FakeEndpoint>>();

            Func<FakeRecipient, IEnumerable<FakeEndpoint>> endpointGetter = r => new[] { endpoint };

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.GetEndpointsUsing(endpointGetter);
            distributor.DeliverUsing(deliverer1.Object);
            distributor.DeliverUsing(deliverer2.Object);

            distributor.DistributeAsync(distributable, recipient).Wait();
            
            deliverer2.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint) endpoint), Times.Once);
        }

        [Test]
        public void DistributeAsync_DistributesMultipleAtATimeByDefault()
        {
            var distributable1 = new FakeDistributable();
            var distributable2 = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));

            Func<FakeRecipient, IEnumerable<FakeEndpoint>> endpointGetter = r => new[] { endpoint };

            distributor.DeliverUsing(deliverer);
            distributor.GetEndpointsUsing(endpointGetter);

            var task1 = distributor.DistributeAsync(distributable1, recipient);
            var task2 = distributor.DistributeAsync(distributable2, recipient);

            Task.WaitAll(task1, task2);

            var lastStartTime = deliverer.LogEntries.Max(e => e.StartDateTime);
            var firstEndTime = deliverer.LogEntries.Min(e => e.EndDateTime);

            lastStartTime.ShouldBeLessThan(firstEndTime);
        }

        [Test]
        public void DistributeAsync_DistributesOneAtATimeWhenThrottledToOne()
        {
            var distributable1 = new FakeDistributable();
            var distributable2 = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));

            Func<FakeRecipient, IEnumerable<FakeEndpoint>> endpointGetter = r => new[] { endpoint };

            distributor.DeliverUsing(deliverer);
            distributor.GetEndpointsUsing(endpointGetter);
            distributor.MaximumConcurrentDeliveries(1);

            var task1 = distributor.DistributeAsync(distributable1, recipient);
            var task2 = distributor.DistributeAsync(distributable2, recipient);

            Task.WaitAll(task1, task2);

            var lastStartTime = deliverer.LogEntries.Max(e => e.StartDateTime);
            var firstEndTime = deliverer.LogEntries.Min(e => e.EndDateTime);

            lastStartTime.ShouldBeGreaterThanOrEqualTo(firstEndTime);
        }

        #region "Fakes"

        public class FakeDistributable : IDistributable
        {
        }

        public class FakeRecipient : IRecipient
        {
        }

        public class FakeEndpoint : IEndpoint
        {
        }

        #endregion "Fakes"
    }
}
