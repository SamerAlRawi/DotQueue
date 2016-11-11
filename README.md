# DotQueue

#### a lightweight queue for .net

#### Features:
- Fully managed
- Concurrent and thread safe
- Pub-Sub is in it's core
- Guarantee sequential order
- Token based authentication
- Minimal code needed to embed in your application
- supports JSON and XML for non dotnet languages
- low latency and event driven subscription 


#### Examples:
[Queue host example](#hostlib)

[Example HostLib(Windows service) using topshelf](#hostlibsvc)

[Queue client(subscriber)](#client)

[Send message to queue](#broadcast)

[Authentication - Queue host](#hostlibauth)

[Authentication - Client subscriber](#clientauth)



#### <a name="hostlib"></a>Example HostLib

Create a new console application
```
Install-Package DotQueue.HostLib
```

```csharp
var httpPort = 8083; //Can be any other port#
var host = new QueueHost(httpPort);
host.Start();
Console.ReadLine();
```

#### <a name="hostlibsvc"></a>HostLib using TopShelf and install as a windows service
Create a new console application
```
Install-Package TopShelf
Install-Package DotQueue.HostLib
```
Copy the following code to your Program.cs
```csharp
private static int _apiPort = 8083;

static void Main(string[] args)
{
    HostFactory.Run(x =>
    {
        x.Service<QueueHost>(s =>
        {
            s.ConstructUsing(factory => new QueueHost(_apiPort));
            s.WhenStarted(tc => tc.Start());
            s.WhenStopped(tc => tc.Stop());
        });
        x.RunAsLocalSystem();

        x.SetDescription("DotQueue QueueHost");
        x.SetDisplayName("DotQueue");
        x.SetServiceName("DotQueue");
    });
}
```
	
To install the service, run the following command using cmd.exe as Admin after building your .exe file

```bash
your_file.exe install
```

### <a name="client"></a>Client MessageQueue Example

Create a new console application

```
Install-Package DotQueue.Client
```

```csharp
public class MyClass{
      public string Email { get; set; }
}
```
```csharp
var queue = new MessageQueue<MyClass>(new DotQueueAddress
   {
       IpAddress = IPAddress.Parse("127.0.0.1"),//IP address or hostname of the queue host
       Port = 8083
   });
/*
below endless loop
subscribe and wait for messages to be published
MessageQueue class implements IEnumerable<>
*/
foreach (var message in queue)
   {
		//Will print any message sent to queue
       Console.WriteLine(message.Email);
   }
```
#### <a name="broadcast"></a>to send a message to the queue


Create a new console application

```
Install-Package DotQueue.Client
```

```csharp
var queue = new MessageQueue<MyClass>(new DotQueueAddress
    {
        IpAddress = IPAddress.Parse("127.0.0.1"),//IP address or hostname of the queue host
        Port = 8083
    });
//Below line will send a new message to the queue
//Any listener will receive the same message
var messageId = queue.Add(new MyClass());
```

#### <a name="hostlibauth"></a>Queue host authentication
Specifying IApiTokenValidator when constructing the QueueHost will require a token specified for all subscribers calls

example implementation of a token validator:
```csharp
public class ApiTokenValidator : IApiTokenValidator
{
   public bool IsValidToken(string token)
   {
      //validate token against your static list or dynamic tokens
      return true; //if token is valid
   }
}
```
injecting validator to QueueHost
```csharp
var validator = new ApiTokenValidator();
//use validator when constructing queue host
var host = new QueueHost(httpPort, validator);
host.Start();
```

#### <a name="clientauth"></a>Queue client authentication
Specifying IApiTokenSource when constructing the MessageQueue will add authentication token to client
example implementation of a token source:
```csharp
public class ApiTokenSource : IApiTokenSource
{
   public string GetToken()
   {
      //generate a token or use a static GUID or string here
      return 'token_secret_here'; 
   }
}
```
injecting token source to MessageQueue

```csharp
var tokenSource = new ApiTokenSource();
//use token source when constructing message queue client
var address = new DotQueueAddress
   {
       IpAddress = IPAddress.Parse("127.0.0.1"),//IP address or hostname of the queue host
       Port = 8083
   }
var queue = new MessageQueue<MyClass>(address, tokenSource);

//send message to queue
queue.Add(new MyClass());

//subscribe and wait for message
foreach(var item in queue){
   //start processing items here
}
```

### Future work:
- _Custom logging_
- _Support clustering_
- _Persistance_
- _Routes and exchanges_
