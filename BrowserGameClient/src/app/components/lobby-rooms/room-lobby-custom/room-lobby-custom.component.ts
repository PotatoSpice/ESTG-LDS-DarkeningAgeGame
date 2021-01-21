import { Component, OnInit } from '@angular/core';

import { SessionService } from 'src/app/services/api/session.service';
import { LobbyCommunicationService } from 'src/app/services/sockets/lobby-communication.service';

@Component({
  selector: 'app-room-lobby-custom',
  templateUrl: './room-lobby-custom.component.html',
  styleUrls: ['./room-lobby-custom.component.css']
})
export class RoomLobbyCustomComponent implements OnInit {

  // player cards list
  players: any[] = [];
  // room identification
  lobbyId: string = "";
  sessionId: string = "";
  hostId: string = "";

  constructor(private lobbyService: LobbyCommunicationService, private sessionService: SessionService) { 
  }

  ngOnInit(): void {
    // Observe changes to the player in session
    this.sessionService.observeSessionPlayer().subscribe(
      (player: any) => {
        if (player) this.sessionId = player.username;
        else this.sessionId = '';
      }
    );
    // Observe room Host
    this.lobbyService.observeRoomHost().subscribe(
      (host: string) => this.hostId = host
    );
    // Observe players in room
    this.lobbyService.observeRoomPlayers().subscribe(
      (players: any[]) => this.players = players
    );
  }

  startGame() {
    this.lobbyService.notifyStartGame();
  }

  ngOnDestroy() {
    // if (this.connected) localStorage.setItem('rp', JSON.stringify(this.players));
  }
}
