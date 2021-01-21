import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { SessionService } from 'src/app/services/api/session.service';
import { LobbyCommunicationService } from 'src/app/services/sockets/lobby-communication.service';
import { ToastService } from 'src/app/services/toast.service';

@Component({
  selector: 'app-game-reconnect',
  templateUrl: './game-reconnect.component.html',
  styleUrls: ['./game-reconnect.component.css']
})
export class GameReconnectComponent implements OnInit {
  // player and room identifications
  playerId: string = "";
  playerToken: string = "";
  gameRoomId: string = "";

  constructor(private lobbyService: LobbyCommunicationService, private sessionService: SessionService,
    private toastService: ToastService, private router: Router) { }

  ngOnInit() {
    this.sessionService.observeSessionPlayer().subscribe(
      (player: any) => {
        const temp = localStorage.getItem('auth-token');
        if (player && player !== '' && temp) {
          this.playerId = player.username;
          this.playerToken = temp;
          console.log("session: ", this.playerId, " - ", this.playerToken)
        }
        else {
          this.playerId = this.playerToken = '';
        };
      }
    );
    this.lobbyService.checkIfInRoom().subscribe(
      (room: string) => { 
        if (room && room === 'SocketError') {
          this.toastService.showToast("Server Problems!", "Game Server could not be reached ...");
          this.router.navigate(['session', 'hub'])
        }
        else if (room !== '') {
          this.gameRoomId = room;
          console.log("game-room: ", this.gameRoomId)
        }
        else {
          this.gameRoomId = '';
        }
      }
    );
  }

  connectToGame() {
    window.open(`DarkeningAgeGame:room-id=${this.gameRoomId}&player-id=${this.playerId}&player-auth=${this.playerToken}`, '_self');
  }

}
