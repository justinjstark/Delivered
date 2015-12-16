using System;

namespace Verdeler
{
    public class FailedDelivery : DeliveryResult
    {
        public Exception Exception { get; private set; }

        public FailedDelivery(DateTime attemptDateTime, Exception exception) : base(attemptDateTime)
        {
            Exception = exception;
        }
    }
}
