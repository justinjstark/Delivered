using Delivered;

namespace DemoDistributor
{
    public class File : IDistributable
    {
        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}
