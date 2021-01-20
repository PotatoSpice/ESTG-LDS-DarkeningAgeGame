using GameWebAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebAPI.Repositories
{
    public interface IGameInviteRepository
    {
        Task<ICollection<GameInvite>> GetGameInvites(string username);
        Task<GameInvite> GetByRoomAndInvited(string invitedId, string roomId);
        Task Create(GameInvite request);
        Task Delete(string invitedId, string roomId);
    }

    public class GameInviteRepository : IGameInviteRepository
    {
        private readonly DatabaseContext _context;

        public GameInviteRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public async Task<ICollection<GameInvite>> GetGameInvites(string username)
        {
            return await _context.GameInvites
                .Where(request => request.invitedId == username).ToListAsync();
        }

        public async Task<GameInvite> GetByRoomAndInvited(string invitedId, string roomId)
        {
            return await _context.GameInvites
                .Where(request => request.invitedId == invitedId && request.roomId == roomId).SingleOrDefaultAsync();
        }

        public async Task Create(GameInvite request)
        {
            _context.GameInvites.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(string invitedId, string roomId)
        {
            var remove = await GetByRoomAndInvited(invitedId, roomId);
            if (remove != null)
            {
                _context.GameInvites.Remove(remove);
            }
            await _context.SaveChangesAsync();
        }
    }
}
