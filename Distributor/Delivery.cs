using System;

namespace Distributor
{
    public class Delivery : IEquatable<Delivery>
    {
        public Guid DistributableId { get; private set; }
        public Guid EndpointId { get; private set; }

        public Delivery(Guid distributableId, Guid endpointId)
        {
            DistributableId = distributableId;
            EndpointId = endpointId;
        }

        public bool Equals(Delivery other)
        {
            if ((object)other == null)
                return false;

            return other.DistributableId == DistributableId && other.EndpointId == EndpointId;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Delivery);
        }

        public static bool operator ==(Delivery delivery1, Delivery delivery2)
        {
            if (delivery1 == null)
                return false;

            return delivery1.Equals(delivery2);
        }

        public static bool operator !=(Delivery delivery1, Delivery delivery2)
        {
            if (delivery1 == null)
                return false;

            return !delivery1.Equals(delivery2);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + DistributableId.GetHashCode();
            hash = (hash * 7) + EndpointId.GetHashCode();

            return hash;
        }
    }
}
