using System;
using System.Linq;
using System.Threading.Tasks;
using F23.StringSimilarity;
using Microsoft.AspNetCore.Mvc;
using Music163API;
using SpotifyAPI.Web;
using spotifyLyrics.Models;

namespace spotifyLyrics.Controllers{

     public class LyricsController : Controller
    {

        IPlayerClient spotifyPlayer;
        public LyricsController(IPlayerClient player)
        {
            spotifyPlayer = player;
        }

        public IActionResult Index(){
            Console.WriteLine("LyricsController called!");
            return View();
        }


        public async Task<ActionResult> Json(String query,String strategy = "NeteaseSong"){
            
            if(spotifyPlayer == null)
                return  Json(new Object());

            ILyricRetrievalStrategy retrievalStrategy;
            switch (strategy)
            {
                case "NeteaseTF":
                    retrievalStrategy = new NeteaseSongSearchTakeFirstLyrics();
                    break;
                case "NeteaseAlbum":
                     retrievalStrategy = new NeteaseAlbumSearchLyrics();
                     break;
                case "NeteaseSong":
                default:
                    retrievalStrategy = new NeteaseSongSearchLyrics();
                    break;
            }
                   
                    try {
                        var cb = await spotifyPlayer.GetCurrentPlayback();
                        if (cb == null || cb.Item == null || !(cb.Item is FullTrack)){
                            return null;
                        }
                        var track = (FullTrack) cb.Item;

                        var lyrics = await retrievalStrategy.retrive(track.Name,track.Artists[0].Name,track.Album.Name);

                         if(lyrics == null){
                            return Json(new Object());
                        }
                        return Json(lyrics.synced_songlines);
                        

                    }catch(APITooManyRequestsException apiException){
                        Console.WriteLine("error fetching spotify");
                        Console.WriteLine("Retry after: "+apiException.RetryAfter);
                        throw apiException;
                    }


           
        }



    }
}