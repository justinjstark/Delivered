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
    public int Port;
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

## Concurrency

When endpoint delivery services are implemented asynchronously (ex: using [FtpClient.Async](https://github.com/rkttu/System.Net.FtpClient.Async)), the deliveries will be performed asynchronously.

```C#
var task1 = distributor.DistributeAsync(someFile1, someVendor);
var task2 = distributor.DistributeAsync(someFile2, someVendor);
Task.WaitAll(task1, task2);
```

Verdeler offers concurrency limitation functionality. There are two places where the maximum concurrency level can be specified.

**1. Distributor Concurrency Limitation**

You can limit the number of concurrent deliveries at the distributor-level.

```C#
distributor.MaximumConcurrentDeliveries(3)
distributor.DistributeAsync(someFile, someVendor).Wait();
```

This will limit the overall concurrent deliveries to three regardless of which endpoint delivery service is used.

**2. Endpoint Delivery Service Concurrency Limitation**

You can also limit the number of concurrent deliveries for each endpoint delivery service.

```C#
public class FtpDeliveryService : EndpointDeliveryService<File, FtpEndpoint>
{
    public FtpDeliveryService()
    {
        MaximumConcurrentDeliveries(3);
        MaximumConcurrentDeliveries(e => e.Host, 1);
    }
    
    public override async Task DoDeliveryAsync(File file, FtpEndpoint ftpEndpoint)
    {
        //Deliver the file to the FTP endpoint
    }
}
```

This will limit the number of concurrent deliveries using FtpDeliveryService to three, but will limit concurrency to one delivery at a time per host. This is useful if you don't want to overly tax a receiving server.

The `MaximumConcurrentDeliveries` with two arguments takes a function with an `Endpoint` parameter and an `object` return. All endpoints are grouped according to the reduction function and `.Equals`. Concurrency limitation is applied to each group. This allows for more complex concurrency limitation such as:

```C#
MaximumConcurrentDeliveries(e => new { e.Host, e.Port }, 1);
```

In this example, FTP deliveries to the same host and port are limited to single concurrency. This would, however, not limit simultaneous deliveries to the same host on two different ports, or to different hosts on the same port.
