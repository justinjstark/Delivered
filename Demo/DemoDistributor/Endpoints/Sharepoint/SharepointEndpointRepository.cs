using System;
using System.Collections.Generic;
using System.Threading;
using Delivered;

namespace DemoDistributor.Endpoints.Sharepoint
{
    public class SharepointEndpointRepository : IEndpointRepository<Vendor>
    {
        public IEnumerable<IEndpoint> GetEndpointsForRecipient(Vendor vendor)
        {
            Console.WriteLine("Getting SharePoint endpoints");

            Thread.Sleep(1000);

            Console.WriteLine("Got SharePoint endpoints");

            return new List<SharepointEndpoint>
            {
                new SharepointEndpoint { Uri = @"http://sharepoint.com/test1" },
                new SharepointEndpoint { Uri = @"http://sharepoint.com/test2" },
                new SharepointEndpoint { Uri = @"http://sharepoint.com/test3" },
                new SharepointEndpoint { Uri = @"http://somethingelse.com/test3" }
            };
        }
    }
}
