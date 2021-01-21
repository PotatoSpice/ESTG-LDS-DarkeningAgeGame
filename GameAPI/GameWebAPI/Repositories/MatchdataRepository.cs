using GameWebAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebAPI.Repositories
{

    public interface IMatchdataRepository
    {
        Task<ICollection<PlayerMatchdata>> GetPlayerMatchdata(string playerId, int nview);
        Task<ICollection<PlayerMatchdata>> GetGameMatchdata(string gameID);
        Task Create(PlayerMatchdata matchdata); 
    }

    public class MatchdataRepository : IMatchdataRepository
    {
        private readonly DatabaseContext _context;
        public MatchdataRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public async Task<ICollection<PlayerMatchdata>> GetPlayerMatchdata(string playerId, int nview)
        {
            return await _context.PlayersMatchdata
                .Where(matches => matches.playerId == playerId).OrderBy(matches => matches.date).Take(nview).ToListAsync();
        }

        public async Task<ICollection<PlayerMatchdata>> GetGameMatchdata(string gameID)
        {
            return await _context.PlayersMatchdata
                .Where(matches => matches.gameID == gameID).ToListAsync();
        }

        public async Task Create(PlayerMatchdata matchdata)
        {
            _context.PlayersMatchdata.Add(matchdata);
            await _context.SaveChangesAsync();
        }


    }
}
