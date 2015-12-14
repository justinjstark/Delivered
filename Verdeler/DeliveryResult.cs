using System;

namespace Verdeler
{
    public abstract class DeliveryResult
    {
        public DateTime AttemptDateTimeUtc;
    }

    public class SuccessfulDelivery : DeliveryResult
    {
    }

    public class FailedDelivery : DeliveryResult
    {
        public Exception Exception;
    }
}
