import { Component, OnInit } from '@angular/core';

import { SessionService } from 'src/app/services/api/session.service';
import { LobbyCommunicationService } from 'src/app/services/sockets/lobby-communication.service';

@Component({
  selector: 'app-room-lobby-matchmaking',
  templateUrl: './room-lobby-matchmaking.component.html',
  styleUrls: ['./room-lobby-matchmaking.component.css']
})
export class RoomLobbyMatchmakingComponent implements OnInit {

  // connection flags
  finding: boolean = false;
  // chat messages and player cards lists
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
    // Observe match finding
    this.lobbyService.observeFindMatch().subscribe(
      (finding: boolean) => {
        if (finding) this.finding = true;
        else this.finding = false;
      }
    );
  }

  findMatch() {
    this.lobbyService.notifyMatchmaking();
  }

  cancelMatchmaking() {
    this.lobbyService.notifyMatchmakingCancel();
  }

  ngOnDestroy() {
    this.lobbyService.leaveLobby();
  }
}
