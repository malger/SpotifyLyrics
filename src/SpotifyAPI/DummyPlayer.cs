using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace SpotifyAPI{
    /**
    Dummy class that is returned if not access token is present
    **/
    public class DummyPlayer : IPlayerClient
    {
        
        public Task<bool> AddToQueue(PlayerAddToQueueRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<DeviceResponse> GetAvailableDevices()
        {
            return Task.FromResult(new DeviceResponse());
        }

        public Task<CurrentlyPlaying> GetCurrentlyPlaying(PlayerCurrentlyPlayingRequest request)
        {
            return Task.FromResult(new CurrentlyPlaying());
        }

        public Task<CurrentlyPlayingContext> GetCurrentPlayback()
        {
            return Task.FromResult(new CurrentlyPlayingContext());
        }

        public Task<CurrentlyPlayingContext> GetCurrentPlayback(PlayerCurrentPlaybackRequest request)
        {
            return Task.FromResult(new CurrentlyPlayingContext());
        }

        public Task<CursorPaging<PlayHistoryItem>> GetRecentlyPlayed()
        {
            return Task.FromResult(new CursorPaging<PlayHistoryItem>());
        }

        public Task<CursorPaging<PlayHistoryItem>> GetRecentlyPlayed(PlayerRecentlyPlayedRequest request)
        {
            return Task.FromResult(new CursorPaging<PlayHistoryItem>());
        }

        public Task<bool> PausePlayback()
        {
            return Task.FromResult(true);
        }

        public Task<bool> PausePlayback(PlayerPausePlaybackRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ResumePlayback()
        {
            return Task.FromResult(true);
        }

        public Task<bool> ResumePlayback(PlayerResumePlaybackRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SeekTo(PlayerSeekToRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetRepeat(PlayerSetRepeatRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetShuffle(PlayerShuffleRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetVolume(PlayerVolumeRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SkipNext()
        {
            return Task.FromResult(true);
        }

        public Task<bool> SkipNext(PlayerSkipNextRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SkipPrevious()
        {
            return Task.FromResult(true);
        }

        public Task<bool> SkipPrevious(PlayerSkipPreviousRequest request)
        {
                        return Task.FromResult(true);

        }

        public Task<bool> TransferPlayback(PlayerTransferPlaybackRequest request)
        {
            return Task.FromResult(true);
        }
    }
}