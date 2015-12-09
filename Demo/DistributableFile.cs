using Verdeler;

namespace Demo
{
    public class DistributableFile : IDistributable
    {
        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}
