using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;

namespace SpotifyAPI{
    class SpotifyPBwatcher : IHostedService
    {

    private readonly ILogger<SpotifyPBwatcher> _logger;
    private Timer _timer;

    private FullTrack lastPBtrack ;
    private bool lastPBplaying ;


    public event EventHandler SongChanged; 
    public event EventHandler PBpaused; 

    public event EventHandler PBPlay; 

    float SECONDS = 3.0f;

        readonly SpotifyClient _spotify;
        public SpotifyPBwatcher(ILogger<SpotifyPBwatcher> logger,SpotifyClient c)
        {
            _spotify = c;
            _logger = logger;
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


        var pbinfo = _spotify.Player.GetCurrentPlayback().Result;
        if (pbinfo == null|| !(pbinfo.Item is FullTrack)) 
            return;
        var pbtrack = (FullTrack) pbinfo.Item;

        if (lastPBtrack==null) {

            lock(this){
                lastPBtrack = pbtrack; //first time init
                lastPBplaying = pbinfo.IsPlaying;
            }
        }

        if (!lastPBtrack.Name.Equals(pbtrack.Name)){
            _logger.LogInformation("song changed");
            _logger.LogInformation(pbtrack.Name);
            lock(this){
                    lastPBtrack = pbtrack;
            }
            OnSongChanged(EventArgs.Empty);
        }
        if (!pbinfo.IsPlaying && lastPBplaying){
            _logger.LogInformation("pb paused");
            lock(this){
                lastPBplaying = false;
            }
            OnPBPause(EventArgs.Empty);
        }
        if (pbinfo.IsPlaying && !lastPBplaying){
            _logger.LogInformation("pb play");
            lock(this){
                lastPBplaying = true;
            }
            OnPBPlay(EventArgs.Empty);
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
    }

    
      protected virtual void OnPBPause(EventArgs e)
    {
        PBpaused?.Invoke(this, e);
    }

    
      protected virtual void OnPBPlay(EventArgs e)
    {
        PBPlay?.Invoke(this, e);
    }

    }
}