# Verdeler

[![Build status](https://ci.appveyor.com/api/projects/status/bc2sduxl2rjwehyo?svg=true)](https://ci.appveyor.com/project/justinjstark/verdeler) [![MIT license](https://img.shields.io/github/license/justinjstark/verdeler.svg)](https://img.shields.io/github/license/justinjstark/verdeler.svg)

A .NET distribution framework supporting custom distributables and endpoints.

## How It Works

Verdeler is designed to coordinate the delivery of files or other custom distributables to recipient endpoints. It is possible for one recipient to have multiple endpoints of various types (FTP, Sharepoint, Web Service, etc). Once Verdeler is configured, calling the `Distribute(distributable, recipient)` method will send the distributable to all endpoints associated with the recipient. Verdeler chooses which endpoint delivery service(s) to use based on the type of the endpoint returned from the registered endpoint repositories.

## Setup

**1. Define a distributable, recipient, and endpoint(s)**

```C#
public class File : Distributable
{
    public string Name;
    public byte[] Contents;
}

public class Vendor : Recipient
{
    public string Name;
}

public class FtpEndpoint : Endpoint
{
    public string Host;
}
```

**2. Define a repository to retrieve endpoints for a recipient**

```C#
public class FtpEndpointRepository : IEndpointRepository<Vendor>
{
    public IEnumerable<Endpoint> GetEndpointsForRecipient(Vendor vendor)
    {
        //Get the FTP endpoints for the vendor
    }
}
```

**3. Define an endpoint delivery service that sends the distributable to the endpoint**

```C#
public class FtpDeliveryService : EndpointDeliveryService<File, FtpEndpoint>
{
    public override async Task DoDeliveryAsync(File file, FtpEndpoint ftpEndpoint)
    {
        //Deliver the file to the FTP endpoint
    }
}
```

**4. Register the repository and delivery service with the distributor and run the distributor**

```C#
var distributor = new Distributor<File, Vendor>();
distributor.RegisterEndpointRepository(new FtpEndpointRepository());
distributor.RegisterEndpointDeliveryService(new FtpDeliveryService());

distributor.DistributeAsync(someFile, someVendor).Wait();
```
