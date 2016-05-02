using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Delivered.Tests
{
    [TestFixture]
    public class EndpointDeliveryServiceTests
    {
        [Test]
        public void DeliverAsync_DeliversMultipleAtATimeByDefault()
        {
            var endpoint = new FakeEndpoint();

            var endpointDeliveryService = new FakeEndpointDeliveryService(new TimeSpan(0, 0, 0, 0, 100));

            var task1 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(new FakeDistributable(), endpoint);
            var task2 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(new FakeDistributable(), endpoint);
        
            Task.WaitAll(task1, task2);

            var lastStartTime = endpointDeliveryService.LogEntries.Max(e => e.StartDateTime);
            var firstEndTime = endpointDeliveryService.LogEntries.Min(e => e.EndDateTime);

            lastStartTime.ShouldBeLessThan(firstEndTime);
        }

        [Test]
        public void DeliverAsync_DeliversOneAtATimeWithConcurrencyLimitOne()
        {
            var distributable1 = new FakeDistributable();
            var distributable2 = new FakeDistributable();
            var endpoint = new FakeEndpoint();
            
            var endpointDeliveryService = new FakeEndpointDeliveryService(new TimeSpan(0, 0, 0, 0, 100));
            endpointDeliveryService.MaximumConcurrentDeliveries(1);

            var task1 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(distributable1, endpoint);
            var task2 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(distributable2, endpoint);

            Task.WaitAll(task1, task2);

            var task1EndTime = endpointDeliveryService.LogEntries.Single(e => e.Distributable == distributable1).EndDateTime;
            var task2StartTime = endpointDeliveryService.LogEntries.Single(e => e.Distributable == distributable2).StartDateTime;

            task1EndTime.ShouldBeLessThanOrEqualTo(task2StartTime);
        }

        [Test]
        public void DeliverAsync_DeliversOneAtATimeWhenLimitedToOneByGroup()
        {
            var distributable = new FakeDistributable();
            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "1" };

            var a = new Dictionary<Func<FakeEndpoint, object>, int>();

            var endpointDeliveryService = new FakeEndpointDeliveryService(new TimeSpan(0, 0, 0, 0, 100),
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

            var task1 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(distributable, endpoint1);
            var task2 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(distributable, endpoint2);

            Task.WaitAll(task1, task2);

            var task1EndTime = endpointDeliveryService.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
            var task2StartTime = endpointDeliveryService.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;

            task1EndTime.ShouldBeLessThanOrEqualTo(task2StartTime);
        }

        [Test]
        public void DeliverAsync_DeliversAtSameTimeWhenEndpointsAreGroupedDifferently()
        {
            var distributable = new FakeDistributable();
            var endpoint1 = new FakeEndpoint { Host = "1" };
            var endpoint2 = new FakeEndpoint { Host = "2" };

            var a = new Dictionary<Func<FakeEndpoint, object>, int>();

            var endpointDeliveryService = new FakeEndpointDeliveryService(new TimeSpan(0, 0, 0, 0, 100),
                new Dictionary<Func<FakeEndpoint, object>, int> { { e => e.Host, 1 } });

            var task1 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(distributable, endpoint1);
            var task2 = ((IEndpointDeliveryService)endpointDeliveryService).DeliverAsync(distributable, endpoint2);

            Task.WaitAll(task1, task2);

            var task1EndTime = endpointDeliveryService.LogEntries.Single(e => e.Endpoint == endpoint1).EndDateTime;
            var task2StartTime = endpointDeliveryService.LogEntries.Single(e => e.Endpoint == endpoint2).StartDateTime;

            task2StartTime.ShouldBeLessThan(task1EndTime);
        }

        #region "Fakes"

        public class FakeEndpointDeliveryService : EndpointDeliveryService<FakeDistributable, FakeEndpoint>
        {
            public class LogEntry
            {
                public FakeDistributable Distributable;
                public FakeEndpoint Endpoint;
                public DateTime StartDateTime;
                public DateTime EndDateTime;
            }

            public List<LogEntry> LogEntries = new List<LogEntry>();

            private readonly TimeSpan _timeSpanToDeliver;

            public FakeEndpointDeliveryService(TimeSpan timeSpanToDeliver)
            {
                _timeSpanToDeliver = timeSpanToDeliver;
            }

            public FakeEndpointDeliveryService(TimeSpan timeSpanToDeliver, IDictionary<Func<FakeEndpoint, object>, int> groupingFuncs)
            {
                _timeSpanToDeliver = timeSpanToDeliver;
                foreach (var keyValuePair in groupingFuncs)
                {
                    MaximumConcurrentDeliveries(keyValuePair.Key, keyValuePair.Value);
                }
            }

            public override async Task DeliverAsync(FakeDistributable distributable, FakeEndpoint endpoint)
            {
                var startTime = DateTime.Now;

                await Task.Delay(_timeSpanToDeliver);

                var endTime = DateTime.Now;

                LogEntries.Add(new LogEntry
                {
                    Distributable = distributable,
                    Endpoint = endpoint,
                    StartDateTime = startTime,
                    EndDateTime = endTime
                });
            }
        }
        
        public class FakeDistributable : IDistributable
        {
        }

        public class FakeEndpoint : IEndpoint
        {
            public string Host { get; set; }
        }

        #endregion "Fakes"
    }
}
