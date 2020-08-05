using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using SpotifyAPI.Web;
using Microsoft.Extensions.Logging;
using spotifyLyrics.Controllers;

namespace spotifyLyrics
{
    public class Startup
    {


        DateTime lastTokenRefresh;

        bool justStarted = true;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        private SpotifyAuthResponse refreshTokenAupdateConfig(String rtoken,String cid,IWritableOptions<SpotifySettings> sett){
         var auth = SpotifyAuthController.refreshToken(rtoken,cid).Result;
            lastTokenRefresh = DateTime.Now;
            sett.Update(opts => opts.Token = auth.access_token);
            sett.Update(opts => opts.RefreshToken = auth.refresh_token);
            return auth;
        }

        public void ConfigureServices(IServiceCollection services)
        {
         
          services.AddLogging();
          services.AddSingleton<SpotifyPKCEauth>();


        services.AddTransient<SpotifyClient>(sprov =>{


                 var spSettings = sprov.GetService<IWritableOptions<SpotifySettings>>();
                var atoken = Configuration["Spotify:Token"];
                var rtoken = Configuration["Spotify:RefreshToken"];
                var cid = Configuration["Spotify:ClientId"];



                //refresh token exists -> execute a refresh at startup
                if (justStarted && rtoken.Length>0) {
                    justStarted = false;
                    var auth = refreshTokenAupdateConfig(rtoken,cid,spSettings);
                    return new SpotifyClient(auth.access_token);
                } 
                //refresh token if time elapsed gt threshold
                if(rtoken.Length>0 && ((DateTime.Now-lastTokenRefresh).Milliseconds > 3200)) { //default token expire time is 3600ms 
                    var auth = refreshTokenAupdateConfig(rtoken,cid,spSettings);
                    return new SpotifyClient(auth.access_token);
                }
                //if there is an access token use it!
                if (atoken.Length>0){
                        return new SpotifyClient(atoken);
                }
                // expect user to login first
                throw new Exception("provide a token first. use login endpoint on spotifyauth controller");

            });

            services.AddSingleton<IHostedService,SpotifyPBwatcher>();

            services.ConfigureWritable<SpotifySettings>(Configuration.GetSection("Spotify"));

            services.AddSingleton<IServiceCollection,ServiceCollection>();

            services.AddControllersWithViews();
          services.AddHttpClient();
          services.AddHealthChecks();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , IHostApplicationLifetime appLifetime)//,IPlayer player,ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
           // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Lyrics}/{action=index}");
           //    endpoints.MapHealthChecks("/health");
            });
            
           //  appLifetime.ApplicationStarted.Register(OnApplicationStartedAsync(tokenRefresh).Wait);

            BrowserWindowOptions browserWindowOptions = new BrowserWindowOptions();
         //   browserWindowOptions.Frame = false;
           // browserWindowOptions.Transparent = true;
            browserWindowOptions.AlwaysOnTop = true;
                                browserWindowOptions.WebPreferences = new WebPreferences
                    {
                        AllowRunningInsecureContent = true,
                        WebSecurity = false,
                        NodeIntegration = true,
                    };


            
            Task.Run(
                async () => {

                    var b = await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
                    b.WebContents.OpenDevTools();
                }  
            );



        }

    }
}

