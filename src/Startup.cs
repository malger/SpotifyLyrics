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


        bool needSpotifyLogin=false;
        bool startup= true;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }




        public void ConfigureServices(IServiceCollection services)
        {
         
            services.AddLogging();
            services.AddSingleton<SpotifyPKCEauth>();
            services.AddSingleton<SpotifySettings>(sv =>{
                return new SpotifySettings(){
                    Token = Configuration["Spotify:Token"],
                    RefreshToken = Configuration["Spotify:RefreshToken"],
                    ClientId = Configuration["Spotify:ClientId"]

                };
            });
        
            services.ConfigureWritable<SpotifySettings>(Configuration.GetSection("Spotify"));

            services.AddSingleton<IServiceCollection,ServiceCollection>();

            services.AddSingleton<SpotifyPlayerFactory>();
            

            services.AddTransient<IPlayerClient>(svp =>{
                    var spfac = svp.GetService<SpotifyPlayerFactory>();
                    var p=  spfac.GetSpotifyPlayer(startup);
                    startup = startup?false:false;
                    return p;

            });

           services.AddHostedService<SpotifyPBwatcher>();




            services.AddControllersWithViews();
            services.AddHttpClient();
            services.AddHealthChecks();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , IHostApplicationLifetime appLifetime,SpotifyPlayerFactory spfac)//,IPlayer player,ILogger<Startup> logger)
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

            //at startup refresh token
            needSpotifyLogin = !spfac.AccessTokenPresent();

            app.UseEndpoints(endpoints =>
            {
                if(needSpotifyLogin){
                    Console.WriteLine("request auth");
                    endpoints.MapControllerRoute(
                        name: "default",
                    // pattern: "{controller=Lyrics}/{action=index}");
                    pattern: "{controller=SpotifyAuth}/{action=login}");
                } else {
                    Console.WriteLine("skip auth");
                  endpoints.MapControllerRoute(
                    name: "default",
                   // pattern: "{controller=Lyrics}/{action=index}");
                  pattern: "{controller=Lyrics}/{action=index}");
                }

           //    endpoints.MapHealthChecks("/health");
            });
            
           //  appLifetime.ApplicationStarted.Register(OnApplicationStartedAsync(tokenRefresh).Wait);

            BrowserWindowOptions browserWindowOptions = new BrowserWindowOptions();
           browserWindowOptions.Frame = false;
           browserWindowOptions.Transparent = true;
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

