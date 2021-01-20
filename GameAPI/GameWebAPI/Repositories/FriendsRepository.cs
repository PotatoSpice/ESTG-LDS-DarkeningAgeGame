using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWebAPI.Entities;
using GameWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameWebAPI.Repositories
{
    public interface IFriendsRepository
    {
        Task<ICollection<PlayerFriend>> GetFriendsList(string username);
        Task Create(PlayerFriend friend);
        Task Delete(PlayerFriend friendship);
        Task<PlayerFriend> GetByUsernames(string player1Id, string player2Id);
    }

    public class FriendsRepository : IFriendsRepository
    {
        private readonly DatabaseContext _context;

        public FriendsRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public async Task<ICollection<PlayerFriend>> GetFriendsList(string username)
        {
            return await _context.FriendsList
                .Where(friend => friend.friendId == username || friend.playerId == username).ToListAsync();
        }

        public async Task Create(PlayerFriend friend)
        {
            _context.FriendsList.Add(friend);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(PlayerFriend friendship)
        {
            var remove = await GetByUsernames(friendship.playerId, friendship.friendId);
            if(remove != null){
                _context.FriendsList.Remove(remove);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<PlayerFriend> GetByUsernames(string player1Id, string player2Id)
        {
            return await _context.FriendsList
                .Where(friend => (friend.friendId == player1Id && friend.playerId == player2Id) || 
                (friend.friendId == player2Id && friend.playerId == player1Id)).SingleOrDefaultAsync();
        }
    }
}
