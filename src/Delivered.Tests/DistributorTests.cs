using System;
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
        public void DistributeAsync_DeliversToAnEndpointTypeWithNoDeliveryServiceThrowsAnError()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);

            Should.ThrowAsync<InvalidOperationException>(distributor.DistributeAsync(distributable, recipient));
        }

        [Test]
        public void DistributeAsync_DeliversADistributableToARegisteredEndpointDeliveryService()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var endpointDeliveryService = new Mock<IEndpointDeliveryService<FakeDistributable, FakeEndpoint>>();
            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new [] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);
            distributor.RegisterEndpointDeliveryService(endpointDeliveryService.Object);
            
            distributor.DistributeAsync(distributable, recipient).Wait();

            endpointDeliveryService.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint) endpoint), Times.Once);
        }

        [Test]
        public void DistributeAsync_DoesNotUseTheFirstEndpointDeliveryServiceWhenTwoAreRegistered()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var endpointDeliveryService1 = new Mock<IEndpointDeliveryService<FakeDistributable, FakeEndpoint>>();
            var endpointDeliveryService2 = new Mock<IEndpointDeliveryService<FakeDistributable, FakeEndpoint>>();
            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);
            distributor.RegisterEndpointDeliveryService(endpointDeliveryService1.Object);
            distributor.RegisterEndpointDeliveryService(endpointDeliveryService2.Object);

            distributor.DistributeAsync(distributable, recipient).Wait();

            endpointDeliveryService1.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint) endpoint), Times.Never);
        }

        [Test]
        public void DistributeAsync_UsesTheSecondEndpointDeliveryServiceWhenTwoAreRegistered()
        {
            var distributable = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var endpointDeliveryService1 = new Mock<IEndpointDeliveryService<FakeDistributable, FakeEndpoint>>();
            var endpointDeliveryService2 = new Mock<IEndpointDeliveryService<FakeDistributable, FakeEndpoint>>();
            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);
            distributor.RegisterEndpointDeliveryService(endpointDeliveryService1.Object);
            distributor.RegisterEndpointDeliveryService(endpointDeliveryService2.Object);

            distributor.DistributeAsync(distributable, recipient).Wait();
            
            endpointDeliveryService2.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint) endpoint), Times.Once);
        }

        [Test]
        public void DistributeAsync_DistributesMultipleAtATimeByDefault()
        {
            var distributable1 = new FakeDistributable();
            var distributable2 = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();

            var endpointDeliveryService = new FakeLoggedEndpointDeliveryService<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            distributor.RegisterEndpointDeliveryService(endpointDeliveryService);
            distributor.RegisterEndpointRepository(endpointRepository.Object);

            var task1 = distributor.DistributeAsync(distributable1, recipient);
            var task2 = distributor.DistributeAsync(distributable2, recipient);

            Task.WaitAll(task1, task2);

            var lastStartTime = endpointDeliveryService.LogEntries.Max(e => e.StartDateTime);
            var firstEndTime = endpointDeliveryService.LogEntries.Min(e => e.EndDateTime);

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

            var endpointDeliveryService = new FakeLoggedEndpointDeliveryService<FakeDistributable, FakeEndpoint>(new TimeSpan(0, 0, 0, 0, 100));

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            distributor.RegisterEndpointDeliveryService(endpointDeliveryService);
            distributor.RegisterEndpointRepository(endpointRepository.Object);
            distributor.MaximumConcurrentDeliveries(1);

            var task1 = distributor.DistributeAsync(distributable1, recipient);
            var task2 = distributor.DistributeAsync(distributable2, recipient);

            Task.WaitAll(task1, task2);

            var lastStartTime = endpointDeliveryService.LogEntries.Max(e => e.StartDateTime);
            var firstEndTime = endpointDeliveryService.LogEntries.Min(e => e.EndDateTime);

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
