using System.Collections;
using System.Collections.Generic;

namespace Distributor
{
    public sealed class DistributionMethodsCollection : IEnumerable<DistributionMethod>
    {
        private readonly List<DistributionMethod> _distributionMethods = new List<DistributionMethod>();

        public void Add(DistributionMethod distributor)
        {
            _distributionMethods.Add(distributor);
        }

        public IEnumerator<DistributionMethod> GetEnumerator()
        {
            return _distributionMethods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
