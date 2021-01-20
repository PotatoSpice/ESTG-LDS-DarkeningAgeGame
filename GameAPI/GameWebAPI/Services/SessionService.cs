using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GameWebAPI.Services
{
    public interface ISessionService
    {
        Task<Player> SignIn(string username, string password);

        Task<Player> SignInWithEmail(string email, string password);

        Task<Player> SignUp(Player player, string password);

        Task<Player> CreatePasswordResetToken(string email);

        Task<Player> UpdatePasswordReset(string resetToken, string password);
    }
    
    public class SessionService : ISessionService
    {
        private readonly IPlayerRepository _repository;

        public SessionService(IPlayerRepository repository)
        {
            this._repository = repository;
        }

        public async Task<Player> SignIn(string username, string password)
        {
            var found = await _repository.GetByUsername(username);
            if (found == null)
                throw new EntityNotFoundException("Player");

            if (!VerifyHashedPassword(password, found.passwordHash, found.passwordSalt))
                throw new InvalidPasswordException();

            return found;
        }

        public async Task<Player> SignInWithEmail(string email, string password)
        {
            var found = await _repository.GetByEmail(email);
            if (found == null)
                throw new EntityNotFoundException("Player");

            if (!VerifyHashedPassword(password, found.passwordHash, found.passwordSalt))
                throw new InvalidPasswordException();
            
            return found;
        }

        public async Task<Player> SignUp(Player player, string password)
        {
            var found_usrname = await _repository.GetByUsername(player.username);
            if (found_usrname != null)
                throw new AlreadyExistsUsernameException(player.username);
            
            var found_email = await _repository.GetByEmail(player.email);
            if (found_email != null)
                throw new AlreadyExistsEmailException(player.email);

            // hash password before saving
            byte[] hash, salt;
            HashPassword(password, out hash, out salt);
            player.passwordHash = hash;
            player.passwordSalt = salt;
            player.online = false;

            return await _repository.Create(player);
        }

        public async Task<Player> CreatePasswordResetToken(string email)
        {
            var player = await _repository.GetByEmail(email);
            if (player == null) return null;
            Console.WriteLine("[SessionService] found email: " + player.email);

            // create reset token that expires after a time period
            player.pwdResetToken = RandomTokenString();
            player.pwdResetTokenExpires = DateTime.UtcNow.AddMinutes(15);
            Console.WriteLine("[SessionService] made token: " + player.pwdResetToken);

            return await _repository.Update(player);
        }

        public async Task<Player> UpdatePasswordReset(string resetToken, string password)
        {
            var player = await _repository.GetByPasswordResetToken(resetToken);
            if (player == null)
                throw new InvalidPasswordException(resetToken);
            Console.WriteLine("[SessionService] found token player: " + player.email);
            
            // hash password before saving
            byte[] hash, salt;
            HashPassword(password, out hash, out salt);
            player.passwordHash = hash;
            player.passwordSalt = salt;
            
            player.pwdResetToken = null;
            player.pwdResetTokenExpires = null;
            Console.WriteLine("[SessionService] new password: " + password);
            
            return await _repository.Update(player);
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

        private string RandomTokenString()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[40];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                // convert random bytes to hex string
                return BitConverter.ToString(randomBytes).Replace("-", "");
            }
        }
    }
}