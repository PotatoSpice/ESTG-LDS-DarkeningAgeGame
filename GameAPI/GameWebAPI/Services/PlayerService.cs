using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models.Player;
using GameWebAPI.Repositories;

namespace GameWebAPI.Services
{
    public interface IPlayerService
    {
        Task<ICollection<Player>> GetPlayers();

        Task<Player> GetByEmail(string email);

        Task<Player> GetByUsername(string username);

        Task<Player> Update(string username, UpdatePlayerRequest updateInfo);

        Task Delete(Player player);

        Task<Player> ChangeUserStatus(Player player, PlayerStatusRequest plauerStatus);
    }

    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repository;

        public PlayerService(IPlayerRepository repository)
        {
            this._repository = repository;
        }

        public async Task<ICollection<Player>> GetPlayers()
        {
            return await _repository.GetAll();
        }

        public async Task<Player> GetByEmail(string email)
        {
            Player player = await _repository.GetByEmail(email);
            if (player == null)
                throw new EntityNotFoundException();
            return player;
        }

        public async Task<Player> GetByUsername(string username)
        {
            Player player = await _repository.GetByUsername(username);
            if (player == null)
                throw new EntityNotFoundException();
            return player;
        }

        public async Task Delete(Player player)
        {
            var found = await _repository.GetByEmail(player.email);
            if (found == null)
                throw new EntityNotFoundException();
            await _repository.Delete(player);
        }

        public async Task<Player> Update(string username, UpdatePlayerRequest updateInfo)
        {
            var found = await _repository.GetByUsername(username);
            if(VerifyHashedPassword(updateInfo.currentPassword, found.passwordHash, found.passwordSalt)){
                if (found == null)
                    throw new EntityNotFoundException();

                if (!string.IsNullOrWhiteSpace(updateInfo.firstName))
                    found.firstName = updateInfo.firstName;
            
                if (!string.IsNullOrWhiteSpace(updateInfo.lastName))
                    found.lastName = updateInfo.lastName;

                if (!string.IsNullOrWhiteSpace(updateInfo.email))
                    found.email = updateInfo.email;

                if (!string.IsNullOrWhiteSpace(updateInfo.newPassword)){
                    byte[] newHash;
                    byte[] newSalt;
                    HashPassword(updateInfo.newPassword, out newHash, out newSalt);
                    found.passwordSalt = newSalt;
                    found.passwordHash = newHash;
                }

                return await _repository.Update(found);
            }
            else{
                throw new PasswordsDontMatch();
            }
        }

        public async Task<Player> ChangeUserStatus(Player player, PlayerStatusRequest playerStatus)
        {
            player.online = playerStatus.status;
            return await _repository.Update(player);
        }

        private static bool VerifyHashedPassword(string password, byte[] hashToCheck, byte[] hashSalt)
        {
            if (password == null) 
                throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) 
                throw new ArgumentException("Value cannot be null or whitespace.", "password");
            if (hashToCheck.Length != 64)
                throw new ArgumentException("Password is hashed using HMACSHA512, hash length should be of 64 bytes", "hashToCheck");
            if (hashSalt.Length != 128)
                throw new ArgumentException("Password is hashed using HMACSHA512, salt length should be of 128 bytes", "hashToCheck");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(hashSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hashToCheck[i]) return false;
                }
            }
            return true;
        }

        private static void HashPassword(string password, out byte[] hashed, out byte[] salt)
        {
            if (password == null) 
                throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) 
                throw new ArgumentException("Value cannot be null or whitespace.", "password");
            
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                salt = hmac.Key;
                hashed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}