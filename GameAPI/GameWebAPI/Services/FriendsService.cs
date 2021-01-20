using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models;
using GameWebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameWebAPI.Services
{

    public interface IFriendsService
    {
        Task<ICollection<Player>> GetFriendsList(string username);
        Task SendFriendInvite(string player, string targetPlayer);
        Task<ICollection<FriendInvite>> GetFriendInvites(string username);
        Task AcceptFriendInvite(FriendInvite sendRequest, bool accept);
        Task UnfriendPlayer(string player, string targetPlayer);
    }

    public class FriendsService : IFriendsService
    {
        private readonly IFriendInviteRepository _requestsRepository;
        private readonly IFriendsRepository _friendListRepository;
        private readonly IPlayerRepository _playerRepository;

        public FriendsService(IFriendInviteRepository requestsRepository, IFriendsRepository friendListRepository, IPlayerRepository playerRepository)
        {
            this._requestsRepository = requestsRepository;
            this._friendListRepository = friendListRepository;
            this._playerRepository = playerRepository;
        }

        public async Task<ICollection<FriendInvite>> GetFriendInvites(string username)
        {
            return await _requestsRepository.GetFriendInvites(username);
        }

        public async Task<ICollection<Player>> GetFriendsList(string username)
        {
            ICollection<Player> friendList = new List<Player>();

            ICollection<PlayerFriend> friends = await _friendListRepository.GetFriendsList(username);
            foreach(PlayerFriend f in friends)
            {
                string friendUsername;
                if(f.friendId != username)
                {
                    friendUsername = f.friendId;
                }
                else
                {
                    friendUsername = f.playerId;
                }
                Player friend = await _playerRepository.GetByUsername(friendUsername);
                friendList.Add(friend);
            }
            return friendList;
        }

        public async Task AcceptFriendInvite(FriendInvite sendRequest, bool accept)
        {
            Player player = await _playerRepository.GetByUsername(sendRequest.targetPlayerId);
            if (player == null)
            {
                throw new EntityNotFoundException();
            }
            FriendInvite request = await _requestsRepository.GetByUsernames(sendRequest.playerId, sendRequest.targetPlayerId);
            if (request == null)
            {
                throw new FriendRequestNotFound();
            }
            if (accept)
            {
                PlayerFriend friend = new PlayerFriend(sendRequest.playerId, sendRequest.targetPlayerId);
                await _friendListRepository.Create(friend);
            }
            await _requestsRepository.Delete(sendRequest);
        }

        public async Task SendFriendInvite(string playerId, string targetPlayerId)
        {
            if(playerId == targetPlayerId){
                throw new SendToYourselfException();
            }

            Player player = await _playerRepository.GetByUsername(targetPlayerId);
            if (player == null)
            {
                throw new EntityNotFoundException();
            }

            PlayerFriend foundFriend = new PlayerFriend(playerId, targetPlayerId);
            foundFriend = await _friendListRepository.GetByUsernames(foundFriend.playerId, foundFriend.friendId);
            if (foundFriend != null)
            {
                throw new AlreadyFriendsException();
            }
            FriendInvite foundSendRequest = await _requestsRepository.GetByUsernames(playerId, targetPlayerId);
            FriendInvite foundReceiveRequest = await _requestsRepository.GetByUsernames(targetPlayerId, playerId);
            if (foundSendRequest != null || foundReceiveRequest != null)
            {
                throw new AlreadyExistsFriendRequest();
            }

            FriendInvite createRequest = new FriendInvite();
            createRequest.playerId = playerId;
            createRequest.targetPlayerId = targetPlayerId;
            await _requestsRepository.Create(createRequest);
        }

        public async Task UnfriendPlayer(string playerId, string targetPlayerId)
        {
            Player player = await _playerRepository.GetByUsername(targetPlayerId);
            if (player == null)
            {
                throw new EntityNotFoundException();
            }
            PlayerFriend foundFriend = new PlayerFriend(playerId, targetPlayerId);
            foundFriend = await _friendListRepository.GetByUsernames(foundFriend.playerId, foundFriend.friendId);
            if (foundFriend == null)
            {
                throw new FriendNotFoundException();
            }
            await _friendListRepository.Delete(foundFriend);
        }
    }
}
