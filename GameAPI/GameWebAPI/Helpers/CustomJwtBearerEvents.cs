using System;
using GameWebAPI.Entities;
using GameWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace GameWebAPI.Helpers
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        public CustomJwtBearerEvents() : base()
        {
            base.OnTokenValidated = async (context) => 
            {
                var playerService = context.HttpContext.RequestServices.GetRequiredService<IPlayerService>();
                try
                {
                    Player player = await playerService.GetByUsername(context.Principal.Identity.Name);
                    context.HttpContext.Items["SessionPlayer"] = player;
                }
                catch (Exception)
                {
                    context.Fail("Unauthorized");
                }
            };
        }
    }
}