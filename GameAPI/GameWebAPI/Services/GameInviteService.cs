using System.Collections.Generic;
using System.Threading.Tasks;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models.GameInvite;
using GameWebAPI.Repositories;

namespace GameWebAPI.Services
{
    public interface IGameInviteService
    {
        Task<ICollection<GameInvite>> GetPlayerGameInvites(string username);
        Task SendGameInvite(GameInvite inviteRequest);
        Task DeleteGameInvite(string invitedId, string roomId);
    }

    public class GameInviteService : IGameInviteService
    {
        private readonly IGameInviteRepository _gameInviteRepository;
        private readonly IFriendsRepository _friendListRepository;
        private readonly IPlayerRepository _playerRepository;

        public GameInviteService(IGameInviteRepository gameInviteRepository, IFriendsRepository friendListRepository, IPlayerRepository playerRepository)
        {
            this._gameInviteRepository = gameInviteRepository;
            this._friendListRepository = friendListRepository;
            this._playerRepository = playerRepository;
        }

        public async Task<ICollection<GameInvite>> GetPlayerGameInvites(string username)
        {
            return await _gameInviteRepository.GetGameInvites(username);
        }

        public async Task SendGameInvite(GameInvite inviteRequest)
        {
            if(inviteRequest.hostId == inviteRequest.invitedId){
                throw new SendToYourselfException();
            }

            Player player = await _playerRepository.GetByUsername(inviteRequest.invitedId);
            if (player == null)
            {
                throw new EntityNotFoundException();
            }

            PlayerFriend foundFriend = new PlayerFriend(inviteRequest.hostId, inviteRequest.invitedId);
            foundFriend = await _friendListRepository.GetByUsernames(foundFriend.playerId, foundFriend.friendId);
            if (foundFriend == null)
            {
                throw new FriendNotFoundException();
            }

            GameInvite foundInvite = await _gameInviteRepository.GetByRoomAndInvited(inviteRequest.invitedId, inviteRequest.roomId);
            if(foundInvite != null){
                throw new AlreadyExistsGameInvite();
            }
            await _gameInviteRepository.Create(inviteRequest);
        }

        public async Task DeleteGameInvite(string invitedId, string roomId)
        {
            GameInvite foundInvite = await _gameInviteRepository.GetByRoomAndInvited(invitedId, roomId);
            if(foundInvite == null){
                throw new GameInviteNotFoundException();
            }
            await _gameInviteRepository.Delete(invitedId, roomId);
        }
    }
}