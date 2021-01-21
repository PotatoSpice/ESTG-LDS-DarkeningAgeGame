import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { LobbyCommunicationService } from 'src/app/services/sockets/lobby-communication.service';
import { FriendsService } from 'src/app/services/api/friends.service';
import { ToastService } from 'src/app/services/toast.service';

import { ErrorMessage } from 'src/app/models/ErrorMessage';
import { LobbyAlert } from 'src/app/models/LobbyAlert';
import { LobbyAlertExtended } from 'src/app/models/LobbyAlertExtended';
import { ChatMessage } from 'src/app/models/ChatMessage';

@Component({
  selector: 'app-room-lobby',
  templateUrl: './room-lobby.component.html',
  styleUrls: ['./room-lobby.component.css']
})
export class RoomLobbyComponent implements OnInit {

  // connection flags
  ingame: boolean = false;
  connected: boolean = false;
  connError: boolean = false; 
  // chat messages
  messages: ChatMessage[] = [];
  // room identification
  lobbyId: string = "";

  constructor(private lobbyService: LobbyCommunicationService, private toastService: ToastService, 
    private friendsService: FriendsService, private router: Router, private route: ActivatedRoute) { 
  }

  ngOnInit(): void {
    // Observe new chat messages
    this.lobbyService.observeChatMessages().subscribe(
      (message: ChatMessage | null) => {
        if (message) this.messages.push(message);
      }
    );
    // Update friend list
    this.friendsService.refreshFriendsList().subscribe();
    // host, join or reenter a room
    let currentLobby = this.lobbyService.getLobbyRoomId();
    if (currentLobby !== '') {
      // return to a previous room connection
      this.lobbyId = currentLobby;
      this.connectAndJoin(currentLobby);
      this.connected = true;
    } 
    else {
      // Host or Join a room. Page only loads if Host or Join is given
      this.route.queryParams.subscribe((params) => { 
        if (params.action && params.action === 'host') {
          if (this.router.url.includes('/find-match')) {
            this.connectAndHost(true);
          } else {
            this.connectAndHost(false);
          }
        } 
        else if (params.action && params.action === 'join') {
          if (this.router.url.includes('/find-match')) {
            this.connectAndJoin(params.roomId, true);
          } else {
            this.connectAndJoin(params.roomId, false);
          }
        }
        else this.router.navigate(['/session/hub']);
      });
      // Observe changes for the Lobby Room Id
      this.lobbyService.observeLobbyRoomId().subscribe(
        (lobby: any) => { if (lobby) this.lobbyId = lobby }
      );
    }
  }

  connectAndHost(matchmaking: boolean) {
    this.lobbyService.hostLobby(matchmaking).subscribe(
      (msg: any) => this.handleMessage(msg),
      (err: any) => this.handleSocketError(err),
      () => this.disconnect()
    );
  }

  connectAndJoin(roomId: string, matchmaking?: boolean) {
    this.lobbyService.joinLobby(roomId, matchmaking).subscribe(
      (msg: any) => this.handleMessage(msg),
      (err: any) => this.handleSocketError(err),
      () => this.disconnect()
    )
  }

  private handleSocketError(err: any) {
    this.connError = true;
    console.log('[RoomLobbySocketError]', err);
  }

  private handleMessage(msg: any) {
    this.connError = false;
    this.connected = true;
    if (msg instanceof LobbyAlert) {
      if (msg.title === "game-started") {
        // redirecionar para /session/game-starting
        this.disconnect();
        const data = { state: { 'action': 'redirect' } }
        this.router.navigate(['session', 'game-starting'], data);
      }
    }
    else if (msg instanceof ErrorMessage) {
      if (msg.type === "PlayerInGame") {
        this.ingame = true;
        this.disconnect();
        this.router.navigate(['session' , 'reconnect']);
      } 
      else if (msg.type !== "LobbyWarningException" && msg.type !== "FullRoomException") {
        this.disconnect();
        this.router.navigate(['session' , 'hub']);
      }
      this.toastService.showToast(msg.title, msg.message);
    }
    else if (msg instanceof ChatMessage) {
      // placeholder
    }
    else if (msg instanceof LobbyAlertExtended) {
      // placeholder
    }
    else if (msg === 'SocketError') this.connError = true;
  }

  onMessage(message: string) {
    this.lobbyService.sendChatMessage(message);
  }

  leaveLobby() {
    this.disconnect();
    this.router.navigate(['/session/hub']);
  }

  private disconnect() {
    this.lobbyService.leaveLobby();
    this.connected = false;
    this.messages = [];
  }

  ngOnDestroy() {
    // if (this.connected) localStorage.setItem('rp', JSON.stringify(this.players));
  }
}
