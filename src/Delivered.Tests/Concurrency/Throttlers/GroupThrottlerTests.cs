using Delivered.Concurrency.Throttlers;
using NUnit.Framework;
using Shouldly;

namespace Delivered.Tests.Concurrency.Throttlers
{
    [TestFixture]
    public class GroupThrottlerTests
    {
        [Test]
        public void GetSemaphoreForGroup_GetsSameSemaphoreForSameGrouping()
        {
            var groupThrottler = new GroupThrottler<FakeEndpoint>(e => e.Host, 1);

            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "1" };

            var semaphore1 = groupThrottler.GetSemaphoreForGroup(endpoint1);
            var semaphore2 = groupThrottler.GetSemaphoreForGroup(endpoint2);

            semaphore1.ShouldBe(semaphore2);
        }

        [Test]
        public void GetSemaphoreForGroup_GetsDifferentSemaphoreForDifferentGrouping()
        {
            var groupThrottler = new GroupThrottler<FakeEndpoint>(e => e.Host, 1);

            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "2" };

            var semaphore1 = groupThrottler.GetSemaphoreForGroup(endpoint1);
            var semaphore2 = groupThrottler.GetSemaphoreForGroup(endpoint2);

            semaphore1.ShouldNotBe(semaphore2);
        }

        [Test]
        public void GetSemaphoreForGroup_GetsANullSemaphoreWhenGroupingToNull()
        {
            var groupThrottler = new GroupThrottler<FakeEndpoint>(e => null, 1);

            var endpoint = new FakeEndpoint();

            var semaphore = groupThrottler.GetSemaphoreForGroup(endpoint);

            semaphore.ShouldBeNull();
        }

        public class FakeEndpoint : IEndpoint
        {
            public string Host { get; set; }
        }
    }
}
