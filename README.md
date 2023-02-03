# UptimeKumaHeartbeat

[![CodeQL](https://github.com/JulianusIV/UptimeKumaHeartbeat/actions/workflows/codeql.yml/badge.svg?branch=master)](https://github.com/JulianusIV/UptimeKumaHeartbeat/actions/workflows/codeql.yml)
[![NuGet](https://img.shields.io/nuget/v/UptimeKumaHeartbeat.svg?label=NuGet)](https://nuget.org/packages/UptimeKumaHeartbeat)

## Why does this exist?

If you use [Uptime Kuma](https://github.com/louislam/uptime-kuma) and are making C# services that you want to monitor in Kuma, this is for you.

## How do I set this up?

### In Kuma

Simply create a new monitor and from the Monitor Type Dropdown select the "Push" option.

![grafik](https://user-images.githubusercontent.com/65790187/216563486-81df61fe-11db-446e-95de-ef33c9f8fe5d.png)


If you change the Hearbeat interval be sure to passed the correct interval to the lib (see [parameters](#method-parameters))

### In Code

#### Sending a single Heartbeat

If you want to send a single Hearbeat and want to manage the timing yourself simply call the static method ``HeartbeatManager.SendSingleHeartBeatAsync``, and pass the appropriate [parameters](#method-parameters)

#### Starting regular Heartbeats

If you want to let the library manage the timing for you, create an instance of ``HeartbeatManager`` and call the ``StartHeartbeatsAsync`` method on the object:
```cs
HearbeatManager heartbeatManager = new();
HeartbeatData data = new("", "");
//this is a nonblocking call, and will kick off a background thread sending regular heartbeats
await heartbeatManager.StartHeartbeatsAsync("yoururlhere", data); 
```
Please note that ``HeartbeatManager`` implements ``IDisposable``, and should thus be disposed when no longer needed. Doing so will immediately stop any Heartbeats.

#### Method parameters

##### When sending single Heartbeats the following parameters are required:

``targetUrl``: 
The URL to send the request to.
In your Monitor edit view in Kuma find "Push URL" and copy the link up to (not including) the first parameter.

![grafik](https://user-images.githubusercontent.com/65790187/216561263-8f9fbfc7-8c9e-4116-a471-f5ed8f01c524.png)

``heartbeatData``:
A HeartbeatData object containing a status string, a message string and a ping int.
You can set all of those to empty strings (null in case of the int), unless you want to pass some data to Kuma

##### When starting continuous Heartbeats the following parameters are required:

``targetUrl``:
see previous section

``heartbeatData``:
see previous section

note:
When using continuous Heartbeats you can modify the properties on the object you passed in order to send new data with the next outgoing Heartbeat.

When starting continuous Heartbeats the following parameters are optional:

``cancellationToken``:
You can pass a CT, which when cancelled will immediately stop the Heartbeats and end the background thread.

``interval``:
Time between Heartbeats (in milliseconds), modify if you changed the Heartbeat interval in your [Kuma monitor](#in-kuma)
