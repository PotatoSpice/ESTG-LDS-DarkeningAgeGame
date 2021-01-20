using AutoMapper;
using GameWebAPI.Entities;
using GameWebAPI.Models;
using GameWebAPI.Models.GameInvite;
using GameWebAPI.Models.Player;
using GameWebAPI.Models.PlayerMatchdata;

namespace GameWebAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // base.CreateMap<TSource, TDestination>();
            base.CreateMap<Player, PlayerModel>(); // Map Entity into GET response
            base.CreateMap<SignUpRequest, Player>(); // Map POST request into Entity
            base.CreateMap<Player, PlayerDTO>(); // Map Entity into Auth POST response
            base.CreateMap<Player, PlayerFriendInfo>();
            base.CreateMap<Player, ProfileDTO>();
            base.CreateMap<GameInvite, GameInviteResponse>();
            base.CreateMap<PlayerMatchdata, PlayerMatchdataRequest>() ; 
        }
    }
}