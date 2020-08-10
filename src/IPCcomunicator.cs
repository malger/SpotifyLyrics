using System;
using ElectronNET.API;
using SpotifyAPI;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

class IPCcommunicator  : IHostedService{
    private readonly SpotifyPBwatcher w;

    public IPCcommunicator(SpotifyPBwatcher watcher) => w = watcher;


    public Task StopAsync(CancellationToken cancellationToken)
    {
       return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {

        var b = Electron.WindowManager.BrowserWindows.First();
        EventHandler SongChanged = (s, e) => Electron.IpcMain.Send(b,"song_changed",null);
         EventHandler pbPause = (s, e) => Electron.IpcMain.Send(b,"pause",null);
          EventHandler pbPlay = (s, e) => Electron.IpcMain.Send(b,"play",null);

        w.SongChanged +=SongChanged;
        w.PBPlay+=pbPlay;
        w.PBpaused+=pbPause;

        return Task.CompletedTask;
    }

}