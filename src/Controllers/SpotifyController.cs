using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpotifyAPI;
using SpotifyAPI.Web;
using spotifyLyrics.Models;

namespace spotifyLyrics.Controllers
{

    //public String accessToken = null;



    public class SpotifyController : Controller
    { 


        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                               DateTimeKind.Utc);

        private readonly SpotifyClient _spotify;
        private readonly ILogger<SpotifyController> _logger;

        public SpotifyController(ILogger<SpotifyController> logger, SpotifyClient spotify)
        {
            _spotify = spotify;
            _logger = logger;

        }

        public async Task<string> Song() {
            var pb = await _spotify.Player.GetCurrentPlayback();
            if (pb != null && pb.Item != null && pb.Item is FullTrack)
                return ((FullTrack) pb.Item).Name;
            _logger.LogWarning("no plackback. returning empty string as song");
            return "";
        } 

        public async Task<bool> isPlaying()  {
            var pb = await _spotify.Player.GetCurrentPlayback();
            return pb==null?false:pb.IsPlaying;
            }


        public async Task<DateTime> SongMS() {
            var pb = await _spotify.Player.GetCurrentPlayback();
            return pb!=null?Epoch.AddMilliseconds(pb.ProgressMs):Epoch;

        } 



}

}