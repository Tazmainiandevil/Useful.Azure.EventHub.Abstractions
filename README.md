# Useful.Azure.EventHub.Abstractions

<image src="https://ci.appveyor.com/api/projects/status/github/Tazmainiandevil/Useful.Azure.EventHub.Abstractions?branch=master&svg=true">
<a href="https://badge.fury.io/nu/Useful.Azure.EventHub.Abstractions"><img src="https://badge.fury.io/nu/Useful.Azure.EventHub.Abstractions.svg" alt="NuGet version" height="18"></a>

This library provides a wrapper to help working with Azure EventHub easier

.NET Support:

* .NET Standard 2.0

## Examples

This Data class used for the below examples

```c#
internal class TestData
{
    public int Id { get; set; }
    public string Message { get; set; }
}
```

### Factory

```c#
    const string eventHubName = "<event hub name>";
    const string connectionString = "<event hub connection string>";

    var hub = new EventHubFactory();
```

### Sender

There is a number of senders that are supported from String to a custom object sent as Json.

#### Send String

The send async message allows a single string message to be sent as well as a list.

```c#
    var sender = hub.CreateEventHubSender(connectionString, eventHubName);
    await sender.SendAsync("Test message");
```

#### Send EventData

You can even send an EventData object.

```c#
    var sender = hub.CreateEventHubSender(connectionString, eventHubName);
    await sender.SendAsync(new EventData());
```

#### Send Custom Object

The send as json async message allows a single message to be sent as well as a list.

```c#
    var sender = hub.CreateEventHubSender(connectionString, eventHubName);
    var testMessage = new TestData { Id = "1", Message = "Hello test" };
    await sender.SendAsJsonAsync(testMessage);

```

### Receiver

Receivers can collect some messages or they can collect messages using an Event Processor.

__Note:__ They will collect messages from all partitions.

#### Basic Receiver with message count

```c#
var receiver = hub.CreateEventHubReceiver(connectionString, eventHubName);

var data = receiver.ReceiveAsync(10);
```

#### Receiver with Query and Time

This repeats the query indefinitely for messages within the last x minutes.

```c#
var receiver = hub.CreateEventHubReceiver(connectionString, eventHubName);

receiver.Receive<TestData>(TimeSpan.FromMinutes(20))
                .Repeat()
                .Where(x => x.Message.Contains("some value") && x.Id == 1)
                .Subscribe(x =>
                {
                    Console.WriteLine($"Response - {x.Message}");
                });
```

#### Receiver Processor

This receive method uses an internal Event Processor to capture message and return them to an Observable. When creating the receiver there are Action events for the event messages and error messages.

```c#
    const string storageConnectionString = "<storage connection string>";
    const string storageContainerName = "eventhubcontainer";

    var receiver = hub.CreateEventHubReceiver(connectionString, eventHubName);

    var data = receiver.ReceiveProcessor<TestData>(OnEventError, OnEvent, storageConnectionString, storageContainerName);

    // Process the data from the event hub
    data.Subscribe(x =>
    {
        Console.WriteLine($"Response - {x.Message}");
    });

    // Perform an action with the events being returned e.g. Partition initialised
    private static void OnEvent(string eventArgs)
    {
        Console.WriteLine($"{eventArgs}");
    }

    // Perform an action when an error has occured in the processor
    private static void OnEventError(string eventArgs)
    {
        Console.WriteLine($"Error - {eventArgs}");
    }
```
