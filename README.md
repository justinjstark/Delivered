![Delivered Logo](https://raw.githubusercontent.com/justinjstark/Delivered/master/assets/Delivered.png)

A .NET distribution framework supporting custom distributables and endpoints.

[![Build status](https://ci.appveyor.com/api/projects/status/aycxbupdujefiw80?svg=true)](https://ci.appveyor.com/project/justinjstark/delivered) [![License](https://img.shields.io/github/license/justinjstark/delivered.svg)](https://img.shields.io/github/license/justinjstark/delivered.svg)

## How It Works

Delivered is designed to coordinate the delivery of files or other custom distributables to recipient endpoints. It is possible for one recipient to have multiple endpoints of various types (FTP, Sharepoint, Web Service, etc). Once configured, calling the `Distribute(distributable, recipient)` method will send the distributable to all endpoints associated with the recipient. Delivered chooses which endpoint delivery service(s) to use based on the type of the endpoint returned from the registered endpoint repositories.

## Setup

**1. Define a distributable, recipient, and endpoint(s)**

```C#
public class File : IDistributable
{
    public string Name;
    public byte[] Contents;
}

public class Vendor : IRecipient
{
    public string Name;
}

public class FtpEndpoint : IEndpoint
{
    public string Host;
    public int Port;
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
public class FtpDeliveryService : IEndpointDeliveryService<File, FtpEndpoint>
{
    public async Task DeliverAsync(File file, FtpEndpoint ftpEndpoint)
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

Delivered offers throttling functionality. There are two places where the maximum concurrency level can be specified.

**1. Distributor Throttling**

```C#
distributor.MaximumConcurrentDeliveries(3)
distributor.DistributeAsync(someFile, someVendor).Wait();
```

This will limit the overall concurrent deliveries to three regardless of which endpoint delivery service is used.

**2. Endpoint Delivery Service Throttling**

The abstract class `EndpointDeliveryService` is provided which offers concurrency throttling.

```C#
public class FtpDeliveryService : EndpointDeliveryService<File, FtpEndpoint>
{
    public FtpDeliveryService()
    {
        MaximumConcurrentDeliveries(3);
        MaximumConcurrentDeliveries(e => e.Host, 1);
    }

    public override async Task DeliverAsync(File file, FtpEndpoint ftpEndpoint)
    {
        //Deliver the file to the FTP endpoint
    }
}
```

This will limit the number of all concurrent deliveries using `FtpDeliveryService` to three, but will limit concurrent deliveries per host to one. This is useful if you don't want to overly tax a receiving server.

The second `MaximumConcurrentDeliveries` in the previous example takes a grouping function with an `IEndpoint` parameter and an `object` return. All endpoints are grouped according to the grouping function and `.Equals`. Throttling is applied to each group which allows for a more complex application such as:

```C#
MaximumConcurrentDeliveries(e => new { e.Host, e.Port }, 1);
```

In this example, FTP deliveries to the same host and port are limited to single concurrency. This would, however, not limit simultaneous deliveries to the same host on two different ports, or to different hosts on the same port.

Multiple `MaximumConcurrentDeliveries` can be specified allowing for great flexibility in coordinating deliveries.

```C#
MaximumConcurrentDeliveries(5);
MaximumConcurrentDeliveries(e => e.Host, 3);
MaximumConcurrentDeliveries(e =>
{
    if (e.Host == "reallyslowserver.com")
        return 1;
    else
        return null;
}, 1);
```

In this example, there will be at most five concurrent FTP deliveries. All deliveries to the same host will be limited to three concurrent deliveries. All deliveries to `reallyslowserver.com` will be limited to one concurrent delivery.

Grouping to null will exclude the endpoint from throttling by this group. In the previous example, a delivery to a host other than `reallyslowserver.com` will be limited to five concurrent deliveries and three to the same host but will be unaffected by the third limitation.
