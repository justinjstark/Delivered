using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verdeler
{
    public class SuccessfulDelivery : DeliveryResult
    {
        public SuccessfulDelivery(DateTime attemptDateTime) : base(attemptDateTime)
        {
        }
    }
}
