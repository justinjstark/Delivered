namespace Distributor
{
    public class DistributionFile
    {
        public int Id { get; set; }
        public string ProfileName { get; set; }
        public string FileName { get; set; }
        public byte[] FileContents { get; set; }
    }
}
