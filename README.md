# SynologyDotNet.AudioStation
Synology AudioStation client for .NET.

* Requires [SynologyDotNet.Core](https://www.nuget.org/packages/SynologyDotNet.Core/)
* Targets **.NET Standard 2.0**

## NuGet package

[NuGet package availabe here](https://www.nuget.org/packages/SynologyDotNet.AudioStation/)
```
Install-Package SynologyDotNet.AudioStation
```

## Usage examples

### Basic example with SynoClient

In order to consume data, you may also add other NuGet packages like **SynologyDotNet.AudioStation**.
This example shows how to configure the connection and login with username and password.  

```
// Create an AudioStationClient
var audioStation = new AudioStationClient();

// Create the SynoClient which communicates with the server, this can be re-used across all Station Clients.
var client = new SynoClient(new Uri("https://MySynolgyNAS:5001/"), audioStation);

// Login
await client.LoginAsync("username", "password");

// Get 100 artists from the music library.
var response = await audioStation.ListArtistsAsync(100, 0);
foreach(var artist in response.Data.Artists)
    Console.WriteLine(artist.Name);
```

## SynAudio - Desktop App based on this library

It is a *Synology Audio Station* like desktop application (WPF) for Windows.

[SynAudio on GitHub](https://github.com/geloczigeri/synologydotnet-audiostation-wpf)
