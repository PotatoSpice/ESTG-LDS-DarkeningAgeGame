import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { EMPTY, forkJoin, Observable, of, Subscription } from 'rxjs';
import { catchError, delay, repeat, tap } from 'rxjs/operators';

import { FriendsService } from 'src/app/services/api/friends.service';
import { GameInvite, NotificationsService } from 'src/app/services/api/notifications.service';
import { PlayersService } from 'src/app/services/api/players.service';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {
  pollNotifications?: Subscription;

  notifications: any[] = []
  awaitingResponse: boolean = false;
  inviteTooltip: string = '';
  friendTooltip: string = '';

  constructor(private notificationsService: NotificationsService, private friendsService: FriendsService,
    private playersService: PlayersService, private router: Router) { }

  ngOnInit(): void {
    this.awaitingResponse = true;
    const poll = forkJoin([
      this.notificationsService.getGameNotifications(),
      this.notificationsService.getFriendRequests(),
      this.friendsService.refreshFriendsList()
    ]).pipe(
      tap(([ invites, friends, friendList ]) => this.handleRequest(invites, friends, friendList)),
      catchError((error:any) => this.handleRequestErrors(error)),
      tap(_ => console.info('---searching for new notifications in 5s')),
      delay(5000),
      repeat()
    );
    this.pollNotifications = poll.subscribe();
  }

  acceptFriend(player: any) {
    const index: number = this.notifications.indexOf(player, 0);
    if (index > -1) this.notifications.splice(index, 1);
    this.friendsService.acceptFriendRequest(player, true).subscribe(
      (response: any) => {
        if (response) {
          this.friendsService.refreshFriendsList();
          this.friendTooltip = response;
        }
      } 
    );
  }

  declineFriend(player: any) {
    const index: number = this.notifications.indexOf(player, 0);
    if (index > -1) this.notifications.splice(index, 1);
    this.friendsService.acceptFriendRequest(player, false).subscribe();
  }

  acceptInvite(invite: any) {
    console.log(invite)
    const options = { queryParams: { 'action': 'join', 'roomId': invite.roomId } }
    if (invite.gameType === "Custom") {
      this.router.navigate(['session', 'lobby', 'custom'], options);
    }
    else if (invite.gameType === "Matchmaking") {
      this.router.navigate(['session', 'lobby', 'find-match'], options);
    }
    if (invite.roomId) {
      const index: number = this.notifications.indexOf(invite, 0);
      if (index > -1) this.notifications.splice(index, 1);
      this.notificationsService.revokeGameInvitation(invite.roomId).subscribe();
    }
  }

  declineInvite(invite: any) {
    if (invite.roomId) {
      const index: number = this.notifications.indexOf(invite, 0);
      if (index > -1) this.notifications.splice(index, 1);
      this.notificationsService.revokeGameInvitation(invite.roomId).subscribe();
    }
  }

  private handleRequest(invites: any[], friends: any[], friendList: any) {
    if (invites && friends) {
      let temp: any[] = [];
      this.notifications = temp.concat(invites, friends);
    }
    this.awaitingResponse = false;
  }

  private handleRequestErrors(error: any) {
    this.awaitingResponse = false;
    if (error.status === 500) {
      console.log("[GameNotifications] Something went wrong with the server ...")
    }
    else if (error.status === 401) {
      console.log("[GameNotifications] Player is not signed in ...")
    } 
    else {
      console.log("[GameNotifications] unexpected error: ", error);
    }
    return of(false)
  }

  ngOnDestroy() {
    this.pollNotifications?.unsubscribe();
  }

}
