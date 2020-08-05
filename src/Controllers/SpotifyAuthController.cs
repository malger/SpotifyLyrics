using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using SpotifyAPI.Web;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI;

namespace spotifyLyrics.Controllers{
public class SpotifyAuthController : Controller { //: IHostedService{


        private static readonly HttpClient client = new HttpClient();

        readonly string redirect = "http://localhost:5000/SpotifyAuth";

        private readonly IWritableOptions<SpotifySettings> _spotifySettings;
        private SpotifyPKCEauth pkce;
        public SpotifyAuthController(SpotifyPKCEauth auth,IWritableOptions<SpotifySettings> spotifySettings){
        
            _spotifySettings = spotifySettings;
            this.pkce = auth;

            
        }
        private Uri genAuthUri(String[] scopes){

            
            var req_scopes_enc = Uri.EscapeDataString(string.Join(" ",scopes));

            //TODO: add state param 
            //see https://developer.spotify.com/documentation/general/guides/authorization-guide/#authorization-code-flow-with-proof-key-for-code-exchange-pkce

            var query = new string[]{
                $"client_id={pkce.clientId}",
                $"response_type=code",
                $"redirect_uri={redirect}",
                $"code_challenge={pkce.codeChallenge}",
                $"code_challenge_method=S256",
                $"scopes={req_scopes_enc}"
            };

            UriBuilder ub = new UriBuilder(){
                Scheme = "https",
                Host = "accounts.spotify.com",
                Path = "authorize",
                Query = string.Join("&",query)
                
            };
           return ub.Uri;
        }
    
    public async Task<IActionResult> Auth(String code){
        var values = new Dictionary<string, string>
        {
            { "client_id", pkce.clientId },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "code_verifier", pkce.codeVerifier },
            { "redirect_uri", redirect },


        };

        var content = new FormUrlEncodedContent(values);
        
            //content.Headers.("Content-Type", "application/x-www-form-urlencoded");
        var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);
        response.EnsureSuccessStatusCode();
        var c = await content.ReadAsStringAsync();
        var responseString = await response.Content.ReadAsStringAsync();
        var authobj = JsonConvert.DeserializeObject<SpotifyAuthResponse>(responseString);
        
        _spotifySettings.Update(opt => {
                opt.Token = authobj.access_token;
                opt.RefreshToken = authobj.refresh_token;
        });

        return RedirectToAction("Index","Lyrics",null);
    }


     public IActionResult Index(){
            Console.WriteLine("Spotify Login Registered");
            var scopes = new string[]{
                Scopes.UserReadCurrentlyPlaying,
                Scopes.UserReadPlaybackState,
                Scopes.UserReadPlaybackPosition,
                };
            var uri = this.genAuthUri(scopes);
            return Redirect(uri.AbsoluteUri);

     }

    public async static Task<SpotifyAuthResponse> refreshToken(String refresh_token,String client_id){
        
        var body = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refresh_token },
            { "client_id", client_id },
        };

        var content = new FormUrlEncodedContent(body);
        
            //content.Headers.("Content-Type", "application/x-www-form-urlencoded");
        var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);

        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsStringAsync();
        var authobj = JsonConvert.DeserializeObject<SpotifyAuthResponse>(res);
        return authobj;

    }




    }
}