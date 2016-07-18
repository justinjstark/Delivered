using System;
using System.Linq;
using System.Threading.Tasks;
using Delivered.Tests.Helpers.Fakes;
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

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);

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
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);
            distributor.RegisterDeliverer(deliverer.Object);

            distributor.DistributeAsync(distributable, recipient).Wait();

            deliverer.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint)endpoint), Times.Once);
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

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);
            distributor.RegisterDeliverer(deliverer1.Object);
            distributor.RegisterDeliverer(deliverer2.Object);

            distributor.DistributeAsync(distributable, recipient).Wait();

            deliverer1.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint)endpoint), Times.Never);
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

            var distributor = new Distributor<FakeDistributable, FakeRecipient>();
            distributor.RegisterEndpointRepository(endpointRepository.Object);
            distributor.RegisterDeliverer(deliverer1.Object);
            distributor.RegisterDeliverer(deliverer2.Object);

            distributor.DistributeAsync(distributable, recipient).Wait();

            deliverer2.Verify(eds => eds.DeliverAsync(distributable, (IEndpoint)endpoint), Times.Once);
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
