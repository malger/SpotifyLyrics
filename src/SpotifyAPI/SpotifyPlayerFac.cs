using System;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using spotifyLyrics.Controllers;

namespace SpotifyAPI{
    public class SpotifyPlayerFactory{
        DateTime lastTokenRefresh;
        private SpotifySettings _config;
        private IWritableOptions<SpotifySettings> _wopts;

        public SpotifyPlayerFactory(SpotifySettings config,IWritableOptions<SpotifySettings> wopts)
        {
            _config = config;
            _wopts = wopts;
        }

        public bool AccessTokenPresent() {
            return _config.Token.Length>0;
        }

        private void refreshTokenAupdateConfig(){
   

                lock(this){
                        try{
                if (_config.RefreshToken.Length == 0){
                    Console.WriteLine("no refresh token present");
                    return;
                }

                var auth = SpotifyAuthController.refreshToken(_config.RefreshToken,_config.ClientId).Result;
                if(auth !=null){
                    _config.updateSettings(auth);
                    _wopts.Update(opts => opts.Token = auth.access_token);
                    _wopts.Update(opts => opts.RefreshToken = auth.refresh_token);
                     Console.WriteLine("refreshed token");
                    lastTokenRefresh = DateTime.Now;
                }
            } catch (Exception e){
                // _config.Token = "";
                // _config.RefreshToken ="";
                // _wopts.Update(opts => opts.Token = "");
                // _wopts.Update(opts => opts.RefreshToken = "");
                throw new Exception("token renew failed"); 
            }
                }
                     

        }

        public IPlayerClient GetSpotifyPlayer(bool refresh_token){




                  
            var lasttokenref_time = (DateTime.Now-lastTokenRefresh).Milliseconds;
            Console.WriteLine($"ms since last token refresh {lasttokenref_time}");


            if(!refresh_token)
                refresh_token = lasttokenref_time > 3200?true:false;

            if(refresh_token)
                refreshTokenAupdateConfig();

            String atoken =_config.Token;
            if(atoken==null || atoken.Length == 0){
                return new DummyPlayer();
            }
            var a = new SpotifyClient(atoken);
            return a.Player;
        }

    }
}

