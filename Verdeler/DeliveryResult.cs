using System;

namespace Verdeler
{
    public abstract class DeliveryResult
    {
        public DateTime AttemptDateTime { get; private set; }

        protected DeliveryResult(DateTime attemptDateTime)
        {
            AttemptDateTime = attemptDateTime;
        }
    }
}
