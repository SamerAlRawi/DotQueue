# DotQueue DEPRECATED

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
[Queue Server(HostLib) example](#hostlib)

[Example HostLib(Windows service) using topshelf](#hostlibsvc)

[Queue client(subscriber)](#client)

[Send message to queue](#broadcast)

[Authentication - Queue host](#hostlibauth)

[Authentication - Client subscriber](#clientauth)

[Persistence - Using RavenDB](#persistence_ravendb)

[Persistence - Using SQLite](#persistence_sqlite)

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

#### <a name="persistence_ravendb"></a>Persistence using RavenDB
Specifying IApiTokenSource when constructing the QueueHost will add enable persistence, 
messages will be stored using the specified adapter and messages will be available in case of 
system restart or service restart or update

```sh
Install-Package DotQueue.Persistence.RavenDB
```

example persistance using RavenDB

starta a new Console app and copy the following code to your Main() method, 
same apply for windows service if you are using a [topshelf windows service](#hostlibsvc):
```csharp
var httpPort = 8083; //Can be any other port#
IDocumentStore address = new DocumentStore {
    Url = "http://localhost:8080",
    DefaultDatabase = "Customers",
};
var host = new QueueHost(httpPort, persistenceAdapter:new RavenDbPersistenceAdapter(address));
host.Start();
Console.ReadLine();
```
Note: you can use any RavenDB store, EmbeddableDocumentStore, RavenDbDocumentStore

Or constructing DocumentStore using connection strings from `web.config` 
```csharp
var ravenDbdocumentStore = new DocumentStore
{
    ConnectionStringName = "YOUR_ConnectionStringName"
};
```


#### <a name="persistence_sqlite"></a>Persistence using SQLite
SQLite is a lightweight single file transactional database, 
good candidate for storing thousands to few millions of records

```sh
Install-Package DotQueue.Persistence.SQLite
```

example persistance using SQLite

starta a new Console app and copy the following code to your Main() method, 
same apply for windows service if you are using a [topshelf windows service](#hostlibsvc):
```csharp
var httpPort = 8083; //Can be any other port#

var host = new QueueHost(httpPort, persistenceAdapter:new SQLitePersistenceAdapter());
host.Start();
Console.ReadLine();
```
the data file witll be created in the same location as your executable.

make sure your process have permission to read and right to that directory if you get any File.IO permission errors.

Note: add the following to your `web.confg` if you are running under ASP.net 
Or add it to `app.config` if you are running a windows app to prevent the mismatching framework versions `
Additional information: Mixed mode assembly is built against version 'v2.0.50727' of the runtime and cannot be loaded in the 4.0 runtime without additional configuration information.
`

```xml
<configuration>
    <startup useLegacyV2RuntimeActivationPolicy="true"> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
    </startup>
</configuration>
```

### Future work:
- _Support for clustering and failover_
- _Persistance mongodb, etc.._
- _Routes and exchanges_
