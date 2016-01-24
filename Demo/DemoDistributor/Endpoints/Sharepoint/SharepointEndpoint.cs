using System;
using Delivered;

namespace DemoDistributor.Endpoints.Sharepoint
{
    public class SharepointEndpoint : IEndpoint
    {
        public Guid Id { get; set; }
        public string Uri { get; set; }
    }
}
