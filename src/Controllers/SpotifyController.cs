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

        private readonly ILogger<SpotifyController> _logger;
        private readonly IServiceProvider _services;
        private readonly IPlayerClient _player;

        public SpotifyController(ILogger<SpotifyController> logger, IServiceProvider services,IPlayerClient spotifyplayer)
        {

            _logger = logger;
            _services = services;
            _player = spotifyplayer;

        }

        public async Task<string> Song() {
            try {
                var pb = await _player.GetCurrentPlayback();
                if (pb != null && pb.Item != null && pb.Item is FullTrack)
                    return ((FullTrack) pb.Item).Name;
                return "";
            } catch (Exception e){
                _logger.LogWarning("no plackback. returning empty string as song");
                return "";
            }
           
        } 

        public async Task<bool> isPlaying()  {
            try {
                var pb = await _player.GetCurrentPlayback();
                return pb==null?false:pb.IsPlaying;

            } catch (Exception e){
                return false;
            }
        }


        public async Task<DateTime> SongMS() { 
            try {
                var pb = await _player.GetCurrentPlayback();
                return pb!=null?Epoch.AddMilliseconds(pb.ProgressMs):Epoch;

            } catch (Exception e){
                return Epoch;
            }

        } 



}

}