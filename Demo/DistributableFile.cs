using Verdeler;

namespace Demo
{
    public class DistributableFile : Distributable
    {
        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}
