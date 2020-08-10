using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;

namespace SpotifyAPI{
    class SpotifyPBwatcher : IHostedService
    {

    private readonly ILogger<SpotifyPBwatcher> _logger;
        private readonly IServiceProvider _services;
        private Timer _timer;

    [ThreadStatic] private FullTrack lastPBtrack ;
    [ThreadStatic] private bool lastPBplaying ;


    public event EventHandler SongChanged; 
    public event EventHandler PBpaused; 

    public event EventHandler PBPlay; 



    float SECONDS = 3.0f;

        public SpotifyPBwatcher(ILogger<SpotifyPBwatcher> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _logger.LogInformation("Started Watching Spotify Player state.");
            _logger.LogInformation($"Watching every {SECONDS} seconds");

            _timer = new Timer(WatchPB, null, TimeSpan.Zero,  TimeSpan.FromSeconds(SECONDS));
            return Task.CompletedTask;
        }
     private void WatchPB(object state)
    {

        //dynamically get spotify to have a reference with an up to date token
        var player = _services.GetService<IPlayerClient>();
        try {
        var pbinfo = player.GetCurrentPlayback().Result;
        if (pbinfo == null|| !(pbinfo.Item is FullTrack)) 
            return;
        var pbtrack = (FullTrack) pbinfo.Item;

        if (lastPBtrack==null) {


            lastPBtrack = pbtrack; //first time init
            lastPBplaying = pbinfo.IsPlaying;

        }

        if (!lastPBtrack.Name.Equals(pbtrack.Name)){
            _logger.LogInformation("song changed");
            _logger.LogInformation(pbtrack.Name);
            lastPBtrack = pbtrack;

            OnSongChanged(EventArgs.Empty);
        }
        if (!pbinfo.IsPlaying && lastPBplaying){
            _logger.LogInformation("pb paused");
            lastPBplaying = false;
            OnPBPause(EventArgs.Empty);
        }
        if (pbinfo.IsPlaying && !lastPBplaying){
            _logger.LogInformation("pb play");
            lastPBplaying = true;
            OnPBPlay(EventArgs.Empty);
        } 
        } catch (Exception e){
            _logger.LogError(e.Message);
            return;
        }
        
       
    }


        public Task StopAsync(CancellationToken cancellationToken)
        {
               _logger.LogInformation("Watchin PB stopped.");
              _timer?.Change(Timeout.Infinite, 0);
              return Task.CompletedTask;
        }

        
      protected virtual void OnSongChanged(EventArgs e)
    {
        SongChanged?.Invoke(this, e);
        var b = Electron.WindowManager.BrowserWindows.First();
        Electron.IpcMain.Send(b,"song_changed","");
    }

    
      protected virtual void OnPBPause(EventArgs e)
    {
        PBpaused?.Invoke(this, e);
        var b = Electron.WindowManager.BrowserWindows.First();

        EventHandler pbPause = (s, e) => Electron.IpcMain.Send(b,"pause","");

    }

    
      protected virtual void OnPBPlay(EventArgs e)
    {
        var b = Electron.WindowManager.BrowserWindows.First();

        EventHandler pbPause = (s, e) => Electron.IpcMain.Send(b,"play","");
        PBPlay?.Invoke(this, e);
    }

    }
}