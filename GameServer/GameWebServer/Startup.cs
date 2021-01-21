using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GameWebServer.Services;
using GameWebServer.Middlewares;
using Microsoft.AspNetCore.Http;
using GameWebServer.Repositories;
using System;

namespace GameWebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("ServerPolicy", builder =>
                {
                    builder
                    .AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                    // .WithOrigins(
                    //     Configuration["AppSettings:AllowedOrigins:GameClientOrigin"]
                    //     /* some other needed origins to be allowed */ );
                });
            });

            services.AddHttpClient();

            // # Repositories
            services.AddTransient<IGameRoomManager, GameRoomManager>();
            services.AddTransient<IRoomManager, RoomManager>();
            // services.AddTransient<IConnectionManager, ConnectionManager>();

            // # Services
            services.AddSingleton<IMessageHandlerService, MessageHandlerService>();
            services.AddTransient<IMatchMakingHandlerService, MatchMakingHandlerService>();
            // services.AddSingleton<ILobbyCommunicationService, LobbyCommunicationService>();
            // services.AddSingleton<IGameCommunicationService, GameCommunicationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP Event pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("ServerPolicy");
            app.UseWebSockets();
            // app.Map("/ws-lobby", (app) => app.UseMiddleware<LobbyCommunicationMiddleware>());
            // app.Map("/ws-game", (app) => app.UseMiddleware<GameCommunicationMiddleware>());

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/ws-lobby/{host_join}", 
                    endpoints.CreateApplicationBuilder().UseMiddleware<LobbyMiddleware>().Build());
                endpoints.Map("/ws-game", 
                    endpoints.CreateApplicationBuilder().UseMiddleware<GameMiddleware>().Build());
                
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Game Server is working!");
                });
            });
        }
    }
}
