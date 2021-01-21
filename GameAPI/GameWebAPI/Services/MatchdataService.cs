using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebAPI.Services
{

    public interface IMatchdataService
    {

        Task SavePlayerMatchdata(PlayerMatchdata matchdata);

        Task<ICollection<PlayerMatchdata>> GetPlayerMatchdata(string playerId, int nview);

        Task<ICollection<PlayerMatchdata>> GetGameMatchdata(string roomId); 

    }

    public class MatchdataService : IMatchdataService
    {

        private readonly IMatchdataRepository _matchdataRepository;
        private readonly IPlayerRepository _playerRepository;
            
        public MatchdataService(IMatchdataRepository matchdataRepository, IPlayerRepository playerRepository)
        {
            _matchdataRepository = matchdataRepository;
            _playerRepository = playerRepository; 
        }

        public async Task SavePlayerMatchdata(PlayerMatchdata matchdata)
        {
            await _matchdataRepository.Create(matchdata); 
        }

        public async Task<ICollection<PlayerMatchdata>> GetPlayerMatchdata(string playerId, int nview)
        {
            ICollection<PlayerMatchdata> playerMatches; // = new List<PlayerMatchdata>();
            Player player = await _playerRepository.GetByUsername(playerId);
            if (player == null)
                throw new EntityNotFoundException();

            playerMatches = await _matchdataRepository.GetPlayerMatchdata(playerId, nview);

            return playerMatches; 
        }

        public async Task<ICollection<PlayerMatchdata>> GetGameMatchdata(string roomId)
        {
            ICollection<PlayerMatchdata> matchData; // = new List<PlayerMatchdata>();
            matchData = await _matchdataRepository.GetGameMatchdata(roomId);
            return matchData;
        }


    }
}
