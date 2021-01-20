using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameWebAPI.Repositories
{
    public interface IFriendInviteRepository
    {
        Task<ICollection<FriendInvite>> GetFriendInvites(string username);
        Task Create(FriendInvite request);
        Task Delete(FriendInvite request);

        Task<FriendInvite> GetByUsernames(string requesterUsername, string requestedUsername);
    }

    public class FriendInviteRepository : IFriendInviteRepository
    {
        private readonly DatabaseContext _context;

        public FriendInviteRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public async Task<ICollection<FriendInvite>> GetFriendInvites(string username)
        {
            return await _context.FriendsRequests
                .Where(request => request.targetPlayerId == username).ToListAsync();
        }

        public async Task Create(FriendInvite request)
        {
            _context.FriendsRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(FriendInvite request)
        {
            var remove = await GetByUsernames(request.playerId, request.targetPlayerId);
            if(remove != null){
                _context.FriendsRequests.Remove(remove);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<FriendInvite> GetByUsernames(string requesterUsername, string requestedUsername)
        {
            return await _context.FriendsRequests
                .Where(request => request.playerId == requesterUsername && request.targetPlayerId == requestedUsername).SingleOrDefaultAsync();
        }
    }
}