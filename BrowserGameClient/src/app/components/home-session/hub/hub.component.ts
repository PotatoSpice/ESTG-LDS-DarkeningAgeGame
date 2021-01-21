import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { delay, map, repeat, repeatWhen, retry } from 'rxjs/operators';

import { SessionService } from 'src/app/services/api/session.service';
import { LobbyCommunicationService } from 'src/app/services/sockets/lobby-communication.service';

@Component({
  selector: 'app-hub',
  templateUrl: './hub.component.html',
  styleUrls: ['./hub.component.css']
})
export class HubComponent implements OnInit {

  roomJoin = new FormControl('');
  connected: boolean = false;
  inlobby: boolean = false;
  ingame: boolean = false;
  sessionId: string = '';
  warnings?: string = '';
  
  constructor(private sessionService: SessionService, private lobbyService: LobbyCommunicationService, public router: Router) { 
  }

  ngOnInit(): void {
    // Observe changes to the player in session
    this.sessionService.observeSessionPlayer().subscribe(
      (player: any) => { 
        if (player) this.sessionId = player.username;
        else this.sessionId = '';
      }
    );
    // Check for an open Lobby Room
    this.lobbyService.observeLobbyRoomId().subscribe(
      (room: string) => {
        if (room !== '') {
          this.connected = true;
          this.inlobby = true;
        }
        else this.inlobby = false;
      }
    );
    // Check for an open Game Room
    this.checkServer();
  }

  checkProfile() {
    if (this.sessionId !== '') this.router.navigate(['session', this.sessionId, 'profile']);
  }

  connectAndHost() {
    if (!this.inlobby) {
      const options = { queryParams: { 'action': 'host' } }
      this.router.navigate(['session', 'lobby', 'custom'], options);
    }
  }

  connectAndHostMM() {
    if (!this.inlobby) {
      const options = { queryParams: { 'action': 'host' } }
      this.router.navigate(['session', 'lobby', 'find-match'], options);
    }
  }

  currentlyInLobby() {
    if (this.inlobby) {
      this.router.navigate(['session', 'lobby', 'custom']);
      this.roomJoin.setValue('');
    }
  }

  currentlyInGame() {
    if (this.ingame) {
      // const data = { state: { 'action': 'redirect' } }
      // this.router.navigate(['session', 'game-starting'], data);
      this.router.navigate(['session', 'reconnect']);
      this.roomJoin.setValue('');
    }
  }

  checkServer() {
    if (!this.connected) {
      this.warnings = undefined;
      this.lobbyService.checkIfInRoom().subscribe(
        (resp: string) => {
          if (resp === 'SocketError') {
            this.connected = false;
            this.warnings = "Game Server currently not reachable ... You could try loging in again.";
          } else {
            this.connected = true;
            this.warnings = undefined;
            if (resp !== '') this.ingame = true;
            else this.ingame = false;
          }
        },
        (err: any) => {
          this.warnings = "Something went wrong with the server ...";
        }
      );
    }
  }

  // connectAndJoin() {
  //   if (!this.inlobby) {
  //     const options = { queryParams: { 'action': 'join', 'roomId': this.roomJoin.value } }
  //     this.router.navigate(['session', 'lobby', 'custom'], options);
  //     this.roomJoin.setValue('');
  //   }
  // }

  // connectAndJoinMM() {
  //   if (!this.inlobby) {
  //     const options = { queryParams: { 'action': 'join', 'roomId': this.roomJoin.value } }
  //     this.router.navigate(['session', 'lobby', 'find-match'], options);
  //     this.roomJoin.setValue('');
  //   }
  // }
}
