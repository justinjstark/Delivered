using System;
using NUnit.Framework;
using Shouldly;

namespace Verdeler.UnitTests
{
    [TestFixture]
    public class DeliveryTests
    {
        [Test]
        public void Deliveries_With_Same_Ids_Should_Be_Equal()
        {
            var delivery1DistributableId = Guid.NewGuid();
            var delivery1EndpointId = Guid.NewGuid();
            var delivery1 = new Delivery(delivery1DistributableId, delivery1EndpointId);

            var delivery2DistributableId = new Guid(delivery1DistributableId.ToString()); //Clone
            var delivery2EndpointId = new Guid(delivery1EndpointId.ToString()); //Clone
            var delivery2 = new Delivery(delivery2DistributableId, delivery2EndpointId);

            delivery1.ShouldBe(delivery2);
        }
        
        [Test]
        public void Deliveries_With_Different_Distributable_Ids_Should_Not_Be_Equal()
        {
            var delivery1DistributableId = Guid.NewGuid();
            var delivery1EndpointId = Guid.NewGuid();
            var delivery1 = new Delivery(delivery1DistributableId, delivery1EndpointId);

            var delivery2DistributableId = Guid.NewGuid();
            var delivery2EndpointId = new Guid(delivery1EndpointId.ToString()); //Clone
            var delivery2 = new Delivery(delivery2DistributableId, delivery2EndpointId);

            delivery1.ShouldNotBe(delivery2);
        }
        
        [Test]
        public void Deliveries_With_Different_Endpoint_Ids_Should_Not_Be_Equal()
        {
            var delivery1DistributableId = Guid.NewGuid();
            var delivery1EndpointId = Guid.NewGuid();
            var delivery1 = new Delivery(delivery1DistributableId, delivery1EndpointId);

            var delivery2DistributableId = new Guid(delivery1DistributableId.ToString()); //Clone
            var delivery2EndpointId = Guid.NewGuid();
            var delivery2 = new Delivery(delivery2DistributableId, delivery2EndpointId);

            delivery1.ShouldNotBe(delivery2);
        }

        [Test]
        public void Deliveries_With_Different_Ids_Should_Not_Be_Equal()
        {
            var delivery1 = new Delivery(Guid.NewGuid(), Guid.NewGuid());
            var delivery2 = new Delivery(Guid.NewGuid(), Guid.NewGuid());

            delivery1.ShouldNotBe(delivery2);
        }
    }
}
