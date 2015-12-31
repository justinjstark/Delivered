# Verdeler

[![Build status](https://ci.appveyor.com/api/projects/status/bc2sduxl2rjwehyo?svg=true)](https://ci.appveyor.com/project/justinjstark/verdeler) [![MIT license](https://img.shields.io/github/license/justinjstark/verdeler.svg)](https://img.shields.io/github/license/justinjstark/verdeler.svg)

A .NET distribution framework supporting custom distributables and endpoints.

## Setup

**1. Define a type of distributable**

    public class File : Distributable
    {
        public string Name;
        public byte[] Contents;
    }

**2. Define a type of recipient**

    public class Vendor : Recipient
    {
        public string Name;
    }

**3. Define a type of endpoint**

    public class FtpEndpoint : Endpoint
    {
        public string Host;
    }

**4. Define an endpoint repository**

    public class FtpEndpointRepository : IEndpointRepository<FtpEndpoint>
    {
        public IEnumerable<Endpoint> GetEndpointsForRecipient(Vendor vendor)
        {
            //Get the vendor's FTP endpoints
        }
    }

**5. Define an endpoint delivery service**

    public class FtpDeliveryService : EndpointDeliveryService<File, FtpEndpoint>
    {
        public override async Task DoDeliveryAsync(File file, FtpEndpoint ftpEndpoint)
        {
            //Deliver the file to the FTP endpoint
        }
    }

**6. Register the repository and delivery service with the distributor**

    var distributor = new Distributor<File, Vendor>();
    distributor.RegisterEndpointRepository(new FtpEndpointRepository());
    distributor.RegisterEndpointDeliveryService(new FtpDeliveryService());

**7. Run the distributor**

    distributor.DistributeAsync(someFile, someVendor).Wait();
