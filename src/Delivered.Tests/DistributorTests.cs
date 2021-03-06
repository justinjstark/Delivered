﻿using System;
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

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg => cfg.RegisterEndpointRepository(endpointRepository.Object));
            
            Should.ThrowAsync<InvalidOperationException>(distributor.DistributeAsync(distributable, recipient));
        }

        [Test]
        public void DistributeAsync_DeliversADistributableToARegisteredDeliverer()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var deliverer = new Mock<IDeliverer<FakeDistributable, FakeEndpoint>>();
            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new [] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterEndpointRepository(endpointRepository.Object);
                cfg.RegisterDeliverer(deliverer.Object);
            });
            
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
            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterEndpointRepository(endpointRepository.Object);
                cfg.RegisterDeliverer(deliverer1.Object);
                cfg.RegisterDeliverer(deliverer2.Object);
            });

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
            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterEndpointRepository(endpointRepository.Object);
                cfg.RegisterDeliverer(deliverer1.Object);
                cfg.RegisterDeliverer(deliverer2.Object);
            });

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

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterDeliverer(deliverer);
                cfg.RegisterEndpointRepository(endpointRepository.Object);
            });

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

            var deliverer = new FakeLoggedDeliverer<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterEndpointRepository(endpointRepository.Object);
                cfg.RegisterDeliverer(deliverer);
                cfg.MaximumConcurrentDeliveries(1);
            });

            var task1 = distributor.DistributeAsync(distributable1, recipient);
            var task2 = distributor.DistributeAsync(distributable2, recipient);

            Task.WaitAll(task1, task2);

            var lastStartTime = deliverer.LogEntries.Max(e => e.StartDateTime);
            var firstEndTime = deliverer.LogEntries.Min(e => e.EndDateTime);

            lastStartTime.ShouldBeGreaterThanOrEqualTo(firstEndTime);
        }

        [Test]
        public void DistributeAsync_ReturnsAllExceptions()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint1 = new FakeEndpoint();
            var endpoint2 = new FakeEndpoint();
            var endpoint3 = new FakeEndpoint();

            var deliverer = new ExceptionThrowingDeliverer();

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint1, endpoint2, endpoint3 });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterEndpointRepository(endpointRepository.Object);
                cfg.RegisterDeliverer(deliverer);
                cfg.MaximumConcurrentDeliveries(1);
            });

            Should.ThrowAsync<AggregateException>(async () => await distributor.DistributeAsync(distributable, recipient))
                .Result
                .InnerExceptions
                .Count
                .ShouldBe(3);
        }

        #region "Fakes"

        public class ExceptionThrowingDeliverer : Deliverer<FakeDistributable, FakeEndpoint>
        {
            public override async Task DeliverAsync(FakeDistributable distributable, FakeEndpoint endpoint)
            {
                throw new Exception();
            }
        }

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
