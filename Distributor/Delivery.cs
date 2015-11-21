using System;

namespace Distributor
{
    public class Delivery<TEndpoint> : IEquatable<Delivery<TEndpoint>> where TEndpoint : IEndpoint
    {
        public DistributionFile File { get; set; }
        public TEndpoint Endpoint { get; set; }

        public bool Equals(Delivery<TEndpoint> other)
        {
            if (other == null)
                return false;

            return other.File.Id == File.Id && other.Endpoint.Id == Endpoint.Id;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Delivery<TEndpoint>);
        }

        public static bool operator ==(Delivery<TEndpoint> delivery1, Delivery<TEndpoint> delivery2)
        {
            if (delivery1 == null)
                return false;

            return delivery1.Equals(delivery2);
        }

        public static bool operator !=(Delivery<TEndpoint> delivery1, Delivery<TEndpoint> delivery2)
        {
            if (delivery1 == null)
                return false;

            return !delivery1.Equals(delivery2);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + File.Id.GetHashCode();
            hash = (hash * 7) + Endpoint.Id.GetHashCode();

            return hash;
        }
    }
}
