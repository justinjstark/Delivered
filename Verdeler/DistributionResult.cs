using System.Collections.Generic;
using System.Linq;

namespace Verdeler
{
    public class DistributionResult
    {
        public List<DeliveryResult> DeliveryResults = new List<DeliveryResult>();

        public bool Success()
        {
            return DeliveryResults.All(d => d is SuccessfulDelivery);
        }
    }
}
