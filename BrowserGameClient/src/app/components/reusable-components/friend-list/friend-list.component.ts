import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { FriendInfo, FriendsService } from 'src/app/services/api/friends.service';

import { NotificationsService, SendGameInvite } from 'src/app/services/api/notifications.service';
import { LobbyCommunicationService } from 'src/app/services/sockets/lobby-communication.service';

@Component({
  selector: 'app-friend-list',
  templateUrl: './friend-list.component.html',
  styleUrls: ['./friend-list.component.css']
})
export class FriendListComponent implements OnInit {
  players: any[] = [];
  inroom: boolean = false;
  matchmaking: boolean = false;

  constructor(private notificationsService: NotificationsService, private lobbyService: LobbyCommunicationService,
    private friendsService: FriendsService, private router: Router) { }

  ngOnInit(): void {
    // Change available options depending on page url (Check if in custom or matchmaking room)
    const hasQueryParam = this.router.url.indexOf("?");
    let urlArray;
    if (hasQueryParam != -1) {
      urlArray = this.router.url.substring(0, hasQueryParam).toLocaleLowerCase().split('/');
    } else {
      urlArray = this.router.url.toLocaleLowerCase().split('/');
    }
    if (urlArray.some( path => path === 'lobby')) {
      this.inroom = true;
      if (urlArray.some( path => path === 'find-match')) this.matchmaking = true;
      else this.matchmaking = false;
    }
    else this.inroom = false;
    // Get friends list
    this.friendsService.getFriendsList().subscribe(
      (list: any) => {
        if (list) this.players = list;
      }
    )
  }

  inviteToLobby(player: FriendInfo) {
    if (this.inroom && player.username) {
      this.notificationsService.sendGameInvitation(new SendGameInvite(
        this.lobbyService.getLobbyRoomId(),
        player.username,
        this.matchmaking ? 'Matchmaking' : 'Custom'
      )).subscribe();
    }
  }

  viewPlayerProfile(player: FriendInfo) {
    this.router.navigate(['session', player.username, 'profile']);
  }

  removeFriend(player: FriendInfo) {
    if (player.username) {
      this.friendsService.removeFriend(player.username).subscribe(
        (response: any) => {
          if (response) this.friendsService.refreshFriendsList();
        }
      );
    }
  }

}
