using System;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using GameWebAPI.Helpers;
using GameWebAPI.Repositories;
using GameWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace GameWebAPI
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
            // Database Setup (Sqlite will be used while in Dev, should be changed to SqlServer in Production)
            services.AddDbContext<DatabaseContext>(
                    options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));
                    // options => options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            // MVC & Other Services 
            services.AddCors(options =>
            {
                options.AddPolicy("ServerPolicy", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                    // .WithOrigins( 
                    //     Configuration["AppSettings:AllowedOrigins:GameClientOrigin"],
                    //     Configuration["AppSettings:AllowedOrigins:GameServerOrigin"] );
                });
            });
            services.AddMvc();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            // JWT Auth
            // secret: Configuration["JwtSettings:Secret"], is saved in dotnet user-secrets
            var key = Encoding.ASCII.GetBytes(Configuration["AppSettings:JwtSettings:Secret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.Events = new CustomJwtBearerEvents();
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            // Add Data Access Repositories
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IFriendInviteRepository, FriendInviteRepository>();
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            services.AddScoped<IMatchdataRepository, MatchdataRepository>(); 
            services.AddScoped<IGameInviteRepository, GameInviteRepository>();

            // Add Data Processing Services
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IFriendsService, FriendsService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMatchdataService, MatchdataService>(); 
            services.AddScoped<IGameInviteService,GameInviteService>();

            // Swagger Conf
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("GameWebAPIv1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DarkeningAgeGameWebAPI",
                    Description = "LDS 2020 - Game WebService API in ASP.NET Core 3.1",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Bearer Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(securitySchema, new[] { "Bearer" });
                c.AddSecurityRequirement(securityRequirement);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("ServerPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/GameWebAPIv1/swagger.json", "DarkeningAge WebAPI v1");
                c.RoutePrefix = "docs";
            });
        }
    }
}
