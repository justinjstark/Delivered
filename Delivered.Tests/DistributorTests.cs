using System;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Delivered.Tests
{
    [TestFixture]
    public class DistributorTests
    {
        [Test]
        public void DeliveringToAnEndpointTypeWithNoDeliveryServiceThrowsAnError()
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
        public void OneDistributableCanBeSentToOneEndpointDeliveryService()
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
        public void WhenTwoEndpointDeliveryServicesAreRegisteredForTheSameEndpointTypeTheFirstIsNotUsed()
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
        public void WhenTwoEndpointDeliveryServicesAreRegisteredForTheSameEndpointTypeTheSecondIsUsed()
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

        public class FakeDistributable : IDistributable
        {
        }

        public class FakeRecipient : IRecipient
        {
        }

        public class FakeEndpoint : IEndpoint
        {
        }
    }
}
