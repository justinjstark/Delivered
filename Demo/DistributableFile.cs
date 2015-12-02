using System;
using Distributor;

namespace Demo
{
    public class DistributableFile : IDistributable
    {
        public Guid Id { get; set;  }
        public string RecipientName { get; set; }
        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}
