using System;
using Verdeler;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointEndpoint : Endpoint
    {
        public Guid Id { get; set; }
        public string Uri { get; set; }
    }
}
