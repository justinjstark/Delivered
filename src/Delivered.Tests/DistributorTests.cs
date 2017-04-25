using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Delivered.Tests.Fakes;
using Moq;
using Nito.AsyncEx;
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

            var deliverer = new FakeControlledDeliverer();

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterDeliverer(deliverer);
                cfg.RegisterEndpointRepository(endpointRepository.Object);
            });

            var task1Controller = deliverer.GetController(distributable1, endpoint);
            var task2Controller = deliverer.GetController(distributable2, endpoint);

#pragma warning disable 4014
            distributor.DistributeAsync(distributable1, recipient);
            distributor.DistributeAsync(distributable2, recipient);
#pragma warning restore 4014

            //Both tasks should start simultaneously
            Task.WaitAll(task1Controller.UntilHasStartedAsync(), task2Controller.UntilHasStartedAsync());
        }

        [Test]
        public void DistributeAsync_DistributesOneAtATimeWhenThrottledToOne()
        {
            var distributable1 = new FakeDistributable();
            var distributable2 = new FakeDistributable();
            var recipient = new FakeRecipient();
            var endpoint = new FakeEndpoint();

            var deliverer = new FakeControlledDeliverer();

            var endpointRepository = new Mock<IEndpointRepository<FakeRecipient>>();
            endpointRepository.Setup(e => e.GetEndpointsForRecipient(recipient))
                .Returns(new[] { endpoint });

            var distributor = new Distributor<FakeDistributable, FakeRecipient>(cfg =>
            {
                cfg.RegisterEndpointRepository(endpointRepository.Object);
                cfg.RegisterDeliverer(deliverer);
                cfg.MaximumConcurrentDeliveries(1);
            });

            var task1Controller = deliverer.GetController(distributable1, endpoint);
            var task2Controller = deliverer.GetController(distributable2, endpoint);

            //Start task1
#pragma warning disable 4014
            distributor.DistributeAsync(distributable1, recipient);
#pragma warning restore 4014

            //Wait until task1 has started
            Task.WaitAny(task1Controller.UntilHasStartedAsync(), task2Controller.UntilHasStartedAsync());

            //Start task2
#pragma warning disable 4014
            distributor.DistributeAsync(distributable2, recipient);
#pragma warning restore 4014

            //Wait 2 seconds for task 2 to start
            task2Controller.UntilIsEndingAsync().Wait(2*1000).ShouldBe(false);
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
                await Task.Run(() => { });

                throw new Exception();
            }
        }

        public class FakeControlledDeliverer : Deliverer<FakeDistributable, FakeEndpoint>
        {
            public class Controller
            {
                public bool HasStarted { get; private set; }
                public bool IsEnding { get; private set; }

                private readonly AsyncAutoResetEvent _hasStartedEvent = new AsyncAutoResetEvent();
                private readonly AsyncAutoResetEvent _continueEvent = new AsyncAutoResetEvent();
                private readonly AsyncAutoResetEvent _isEndingEvent = new AsyncAutoResetEvent();
                
                public async Task UntilHasStartedAsync()
                {
                    await _hasStartedEvent.WaitAsync();
                }

                public async Task UntilContinued()
                {
                    await _continueEvent.WaitAsync();
                }

                public async Task UntilIsEndingAsync()
                {
                    await _isEndingEvent.WaitAsync();
                }

                public void Continue()
                {
                    _continueEvent.Set();
                }

                public void MarkStarted()
                {
                    HasStarted = true;
                    _hasStartedEvent.Set();
                }

                public void MarkEnding()
                {
                    IsEnding = true;
                    _isEndingEvent.Set();
                }
            }

            private readonly IDictionary<dynamic, Controller> _controllers = new Dictionary<dynamic, Controller>();
            
            public Controller GetController(FakeDistributable distributable, FakeEndpoint endpoint)
            {
                var key = new { Distributable = distributable, Endpoint = endpoint };

                Controller controller;
                _controllers.TryGetValue(key, out controller);

                if (controller != null) return controller;

                controller = new Controller();
                _controllers.Add(key, controller);

                return controller;
            }

            public override async Task DeliverAsync(FakeDistributable distributable, FakeEndpoint endpoint)
            {
                var controller = GetController(distributable, endpoint);

                controller.MarkStarted();

                await controller.UntilContinued();

                controller.MarkEnding();
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
