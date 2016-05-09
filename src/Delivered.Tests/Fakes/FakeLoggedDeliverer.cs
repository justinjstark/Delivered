using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delivered.Tests.Fakes
{
    public class FakeLoggedDeliverer<TDistributable, TEndpoint> : Deliverer<TDistributable, TEndpoint>
        where TDistributable : IDistributable
        where TEndpoint : IEndpoint
    {
        public class LogEntry
        {
            public TDistributable Distributable;
            public TEndpoint Endpoint;
            public DateTime StartDateTime;
            public DateTime EndDateTime;
        }

        public List<LogEntry> LogEntries = new List<LogEntry>();

        private readonly TimeSpan _timeSpanToDeliver;

        public FakeLoggedDeliverer(TimeSpan timeSpanToDeliver)
        {
            _timeSpanToDeliver = timeSpanToDeliver;
        }

        public FakeLoggedDeliverer(TimeSpan timeSpanToDeliver, IDictionary<Func<TEndpoint, object>, int> groupingFuncs)
        {
            _timeSpanToDeliver = timeSpanToDeliver;
            foreach (var keyValuePair in groupingFuncs)
            {
                MaximumConcurrentDeliveries(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override async Task DeliverAsync(TDistributable distributable, TEndpoint endpoint)
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
}
