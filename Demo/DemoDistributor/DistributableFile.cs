using Delivered;

namespace DemoDistributor
{
    public class DistributableFile : Distributable
    {
        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}
