using System;

namespace Distributor
{
    public class Delivery<TEndpoint> : IEquatable<Delivery<TEndpoint>> where TEndpoint : IEndpoint
    {
        public int FileId { get; private set; }
        public Guid EndpointId { get; private set; }

        public Delivery(int fileId, Guid endpointId)
        {
            FileId = fileId;
            EndpointId = endpointId;
        }

        public bool Equals(Delivery<TEndpoint> other)
        {
            if (other == null)
                return false;

            return other.FileId == FileId && other.EndpointId == EndpointId;
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
            hash = (hash * 7) + FileId.GetHashCode();
            hash = (hash * 7) + EndpointId.GetHashCode();

            return hash;
        }
    }
}
