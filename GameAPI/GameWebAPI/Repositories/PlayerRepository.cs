using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameWebAPI.Repositories
{
    public interface IPlayerRepository
    {
        Task<ICollection<Player>> GetAll();

        Task<Player> GetByEmail(string email);

        Task<Player> GetByUsername(string username);
        
        Task<Player> GetByPasswordResetToken(string token);

        Task<Player> Update(Player player);

        Task<Player> Create(Player player);

        Task Delete(Player player);
    }
    
    public class PlayerRepository : IPlayerRepository
    {
        private readonly DatabaseContext _context;

        public PlayerRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public async Task<ICollection<Player>> GetAll()
        {
            return await _context.Players.AsQueryable().ToListAsync();
        }

        public async Task<Player> GetByEmail(string email)
        {
            return await _context.Players
                .Where(player => player.email == email).SingleOrDefaultAsync();
        }

        public async Task<Player> GetByUsername(string username)
        {
            return await _context.Players
                .Where(player => player.username == username).SingleOrDefaultAsync();
        }

        public async Task<Player> GetByPasswordResetToken(string token)
        {
            return await _context.Players
                .Where(p => p.pwdResetToken == token && p.pwdResetTokenExpires > DateTime.UtcNow)
                .SingleOrDefaultAsync();
        }

        public async Task<Player> Create(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return await this.GetByEmail(player.email);
        }

        public async Task Delete(Player player)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        public async Task<Player> Update(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            return await this.GetByEmail(player.email);
        }
    }
}