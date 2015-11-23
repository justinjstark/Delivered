using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shouldly;

namespace Distributor.UnitTests
{
    [TestFixture]
    public class DeliveryTests
    {
        [Test]
        public void Deliveries_With_Same_Ids_Should_Be_Equal()
        {
            var delivery1 = new Delivery(1, new Guid("2194D9BD-F963-45BD-ACF4-8D071772EE8A"));
            var delivery2 = new Delivery(1, new Guid("2194D9BD-F963-45BD-ACF4-8D071772EE8A"));

            delivery1.ShouldBe(delivery2);
        }

        [Test]
        public void Deliveries_With_Different_File_Ids_Should_Not_Be_Equal()
        {
            var delivery1 = new Delivery(1, new Guid("10B94340-5F75-4C26-954A-9F45CBC34951"));
            var delivery2 = new Delivery(2, new Guid("10B94340-5F75-4C26-954A-9F45CBC34951"));

            delivery1.ShouldNotBe(delivery2);
        }

        [Test]
        public void Deliveries_With_Different_Endpoint_Ids_Should_Not_Be_Equal()
        {
            var delivery1 = new Delivery(1, new Guid("044CA5BD-8443-4666-8B26-406C82A6A651"));
            var delivery2 = new Delivery(1, new Guid("4424CF6E-2414-4340-ACED-32C20A07BFFB"));

            delivery1.ShouldNotBe(delivery2);
        }

        [Test]
        public void Deliveries_With_Different_Ids_Should_Not_Be_Equal()
        {
            var delivery1 = new Delivery(1, new Guid("FF52AFC2-7815-4C90-A853-1BD2FFE913AC"));
            var delivery2 = new Delivery(2, new Guid("4708DA8C-0BA3-40DA-9B79-1CE5446A3E57"));

            delivery1.ShouldNotBe(delivery2);
        }
    }
}
