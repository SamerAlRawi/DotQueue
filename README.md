# DotQueue

#### a lightweight queue for .net

#### Features:
- Fully managed
- Concurrent and thread safe
- Pub-Sub is in it's core
- Authentication
- Persistance
- Minimal code needed to embed in your application

#### Example HostLib

Create a new console application
```
Install-Package DotQueue.HostLib
```

```c
var httpPort = 8083; //Can be any other port#
var host = new QueueHost(httpPort);
host.Start();
Console.ReadLine();
```

#### HostLib using TopShelf and install as a windows service
Create a new console application
```
Install-Package TopShelf
Install-Package DotQueue.HostLib
```
Copy the following code to your Program.cs
```c
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
    Console.Read();
}
```
	
to install the service, run the following command using cmd.exe as Admin after building your .exe file

```bash
your_file.exe install
```

### Client MessageQueue Example

Create a new console application

```
Install-Package DotQueue.Client
```

```c
public class Subscriber{
      public string Email { get; set; }
}
```
```c
var queue = new MessageQueue<Subscriber>(new DotQueueAddress
   {
       IpAddress = IPAddress.Parse("127.0.0.1"),
       Port = 8083
   });
/*
below endless loop
subscribe and wait for messages to be published
MessageQueue class implements IEnumerable<>
*/
foreach (var subscriber in queue)
   {
		//Will print any message sent to queue
       Console.WriteLine(subscriber.Email);
   }
```
#### to send a message to the queue


Create a new console application

```
Install-Package DotQueue.Client
```

```c
var queue = new MessageQueue<Subscriber>(new DotQueueAddress
    {
        IpAddress = IPAddress.Parse("127.0.0.1"),
        Port = 8083
    });
//Below line will send a new message to the queue
//Any listener will receive the same message
var messageId = queue.Add(new Subscriber());
```

WIP:

more example projects.

more sample code.