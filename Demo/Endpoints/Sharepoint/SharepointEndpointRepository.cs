using System;
using System.Collections.Generic;
using System.Threading;
using Verdeler;

namespace Demo.Endpoints.Sharepoint
{
    public class SharepointEndpointRepository : IEndpointRepository<Vendor>
    {
        public IEnumerable<Endpoint> GetEndpointsForRecipient(Vendor vendor)
        {
            Console.WriteLine("Getting SharePoint endpoints");

            Thread.Sleep(1000);

            Console.WriteLine("Got SharePoint endpoints");

            return new List<SharepointEndpoint>
            {
                new SharepointEndpoint { Uri = @"http://sharepoint.com/test1" },
                new SharepointEndpoint { Uri = @"http://sharepoint.com/test2" },
                new SharepointEndpoint { Uri = @"http://somethingelse.com/test3" }
            };
        }
    }
}
