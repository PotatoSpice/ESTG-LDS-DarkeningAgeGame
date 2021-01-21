import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';

import { EMPTY } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { FriendsService } from 'src/app/services/api/friends.service';
import { PlayersService } from 'src/app/services/api/players.service';

import { SessionService } from 'src/app/services/api/session.service';
import { GameRoomCommunicationService } from 'src/app/services/sockets/game-room-communication.service';
import { LobbyCommunicationService } from 'src/app/services/sockets/lobby-communication.service';

@Component({
  selector: 'app-home-session',
  templateUrl: './home-session.component.html',
  styleUrls: ['./home-session.component.css']
})
export class HomeSessionComponent implements OnInit {

  userInSession: any;
  friendForm = new FormControl('');
  sendNotification: string = '';
  
  constructor(private sessionService: SessionService, private lobbyService: LobbyCommunicationService,
    private gameService: GameRoomCommunicationService, private friendsService: FriendsService,
    private playersService: PlayersService, public router: Router) { }

  ngOnInit(): void {
    this.sessionService.observeSessionPlayer().subscribe(
      (userInSession) => {
        this.userInSession = userInSession;
        if (!this.userInSession) { // # redirect to login if no user is in session
          if (this.sessionService.expired ) {
            const options = { queryParams: { expired: 'true' } };
            this.router.navigate(['/sign-in'], options);
          } else {
            this.router.navigate(['/sign-in']);
          }
        } else {
          this.playersService.setPlayerStatus(true).subscribe();
        }
      }
    )
  }

  sendFriendRequest() {
    if (this.friendForm.valid) {
      this.friendsService.sendFriendRequest(this.friendForm.value).subscribe(
        (response: any) => {
          if (response) this.sendNotification = response;
        } 
      );
    }
  }

  signOut() {
    this.playersService.setPlayerStatus(false).subscribe();
    this.sessionService.signOut();
    this.lobbyService.leaveLobby();
    this.gameService.leaveBrowser();
    localStorage.clear();
  }

}
