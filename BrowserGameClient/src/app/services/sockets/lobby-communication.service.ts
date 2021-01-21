import { Injectable } from '@angular/core';

import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';

import { SocketConnectionService } from './socket-connection.service';
import { SessionService } from '../api/session.service';

import { ChatMessage } from '../../models/ChatMessage';
import { LobbyAlert } from '../../models/LobbyAlert';
import { LobbyAlertExtended } from '../../models/LobbyAlertExtended';
import { ErrorMessage } from '../../models/ErrorMessage';

const API_ENDPOINT = environment.gameUrl + "/ws-lobby";

@Injectable({
  providedIn: 'root'
})
export class LobbyCommunicationService {
  
  private lobbyRoomId: BehaviorSubject<string>;
  private gameRoomId: BehaviorSubject<string>;

  private players: any[];
  private roomPlayers: BehaviorSubject<any[]>;
  private roomHost: BehaviorSubject<string>;
  private findingMatch: BehaviorSubject<boolean>;
  private chatMessages: BehaviorSubject<ChatMessage | null>;

  private matchmaking: boolean = false;
  private connected: boolean = false;
  
  constructor(private wsConnectionService: SocketConnectionService, private sessionService: SessionService) {
    let tempLobby = localStorage.getItem('lr');
    this.lobbyRoomId = new BehaviorSubject<string>(tempLobby ? tempLobby : '');
    this.gameRoomId = new BehaviorSubject<string>('');
    this.roomHost = new BehaviorSubject<string>('');
    
    // let tempPlayers = localStorage.getItem('rp');
    this.players = [];
    this.roomPlayers = new BehaviorSubject<any[]>(this.players);
      // tempPlayers ? JSON.parse(tempPlayers) : tempPlayers);
    this.findingMatch = new BehaviorSubject<boolean>(false);
    this.chatMessages = new BehaviorSubject<ChatMessage | null>(null);
  }

  public hostLobby(matchmaking?: boolean): Observable<any> {
    this.connected = true;
    if (matchmaking) {
      this.matchmaking = true;
      this.setPlayersArray();
      return this.wsConnectionService.connect(`${API_ENDPOINT}/findmatch`).pipe(
        map((response: MessageEvent): any => {
          return this.mapResponse(response);
        }),
        catchError( (error: any) => this.handleSocketErrors(error) )
      );
    } else {
      this.matchmaking = false;
      this.setPlayersArray();
      return this.wsConnectionService.connect(`${API_ENDPOINT}/host`).pipe(
        map((response: MessageEvent): any => {
          return this.mapResponse(response);
        }),
        catchError( (error: any) => this.handleSocketErrors(error) )
      );
    }
  }

  public joinLobby(lobbyId: string, matchmaking?: boolean): Observable<any> {
    if (!this.connected) {
      if (matchmaking) this.matchmaking = true;
      else this.matchmaking = false;
      this.setPlayersArray();

      localStorage.setItem("lr", lobbyId);
      this.lobbyRoomId.next(lobbyId);
      this.connected = true;
    }
    return this.wsConnectionService.connect(`${API_ENDPOINT}/join/?room-id=${lobbyId}`).pipe(
      map((response: MessageEvent): any => {
        return this.mapResponse(response);
      }),
      catchError( (error: any) => this.handleSocketErrors(error) )
    );
  }

  public checkIfInRoom(): Observable<string> {
    return this.wsConnectionService.connect(`${API_ENDPOINT}/join`).pipe(
      map((response: MessageEvent): any => {
        const data = JSON.parse(response.data);
        let res = '';
        if (data.errorType && data.errorType === "PlayerInGame") {
          this.gameRoomId = new BehaviorSubject<string>(data.message);
          res = data.message;
        }
        else if (data.errorType === "AlreadyInLobbyException") {
          this.lobbyRoomId.next(data.message);
        }
        this.wsConnectionService.closeConnection();
        return res;
      }),
      catchError( (error: any) => this.handleSocketErrors(error) )
    );
  }

  private handleSocketErrors(err: any): Observable<any> {
    console.log('[ServiceCaughtSocketError]', err);
    if (this.wsConnectionService.closeConnection()) {
      this.lobbyRoomId.next('');
      this.gameRoomId.next('');
      this.findingMatch.next(false);
    }
    this.connected = false;
    return of('SocketError');
  }

  public sendChatMessage(message: string): boolean {
    if (this.connected && message && message.trim()) {
      this.wsConnectionService.sendMessage({ eventType: "chat-message", data: message });
      return true;
    } else return false;
  }

  public notifyMatchmaking() {
    if (this.connected && this.matchmaking) 
      this.wsConnectionService.sendMessage({ eventType: "start-matchmaking"});
  }

  public notifyMatchmakingCancel() {
    if (this.connected && this.matchmaking) 
      this.wsConnectionService.sendMessage({ eventType: "cancel-matchmaking"});
  }

  public notifyStartGame() {
    if (this.connected && !this.matchmaking) 
      this.wsConnectionService.sendMessage({ eventType: "start-game"});
  }

  public leaveLobby() {
    if (this.connected && this.wsConnectionService.closeConnection()) {
      localStorage.removeItem('lr');
      // localStorage.removeItem('rp');
      this.connected = false;
      this.lobbyRoomId.next('');
      this.players = [];
      this.chatMessages.next(null);
    }
  }

  public observeChatMessages(): Observable<ChatMessage | null> {
    return this.chatMessages.asObservable();
  }

  public observeRoomPlayers(): Observable<any[]> {
    return this.roomPlayers.asObservable();
  }

  public observeFindMatch(): Observable<boolean> {
    return this.findingMatch.asObservable();
  }

  public observeRoomHost(): Observable<string> {
    return this.roomHost.asObservable();
  }
  public getRoomHost(): string {
    return this.roomHost.getValue();
  }

  public observeLobbyRoomId(): Observable<string> {
    return this.lobbyRoomId.asObservable();
  }
  public getLobbyRoomId(): string {
    return this.lobbyRoomId.getValue();
  }

  public observeGameRoomId(): Observable<string> {
    return this.gameRoomId.asObservable();
  }
  public getGameRoomId(): string {
    return this.gameRoomId.getValue();
  }

  private mapResponse(response: MessageEvent) {
    this.connected = true;
    const data = JSON.parse(response.data);
    
    if (data.eventType && data.eventType === "chat-message") {
      const message = new ChatMessage(data.user, data.msg, data.time);
      this.chatMessages.next(message);
      return message;

    }
    else if (data.eventType && data.eventType === "lobby-alert") {
      const alert = new LobbyAlert(data.title, data.data);
      this.handleAlert(alert);
      return alert;

    }
    else if (data.eventType && data.eventType === "lobby-alert-ext") {
      const alertExt = new LobbyAlertExtended(data.title, data.data);
      this.handleAlertExt(alertExt);
      return alertExt;

    }
    else if (data.errorType) {
      let errorTitle = "Uknown Error", message = data.message;
      if (data.errorType === "PlayerInGame") {
        this.gameRoomId.next(data.message);
        errorTitle = 'In-Game!';
      } 
      else if (data.errorType === "LobbyWarningException") {
        errorTitle = 'Warning!';
      } 
      else if (data.errorType === "AlreadyInLobbyException") {
        errorTitle = 'Already in Lobby!';
        message = "You already have an open window for a lobby. Close it to Host or Join a new one!";
      }
      else if (data.errorType === "FullRoomException") {
        errorTitle = 'Lobby Full!';
        message = "The room you're trying to enter is already full.";
      }
      else if (data.errorType === "RoomNotFoundException") {
        errorTitle = 'Something went wrong!';
        message = "The room doesn't exist or has been closed ...";
      }
      else {
        errorTitle = 'Something went wrong!';
      }
      return new ErrorMessage(data.errorType, errorTitle, message);

    }
    else return response.data;
  }

  private handleAlertExt(alert: LobbyAlertExtended) {
    if (alert.title === "lobby-players") {
      // adicionar players à lista de jogadores atualmente no lobby
      alert.data.map( (value, index, array) => {
        this.players[index].in = true; 
        this.players[index].data = value;
      });
      this.roomPlayers.next(this.players);
    }
    else console.log("[LobbyCommunication] Uknown Lobby Extended Alert")
  }

  private handleAlert(alert: LobbyAlert) {
    if (alert.title === "player-connected") {
      // adicionar player à lista de jogadores atualmente no lobby
      this.addPlayerToArray(alert.data);
    } 
    else if (alert.title === "player-disconnected") {
      // remover player da lista de jogadores atualmente no lobby
      this.removePlayerFromArray(alert.data);
    }
    else if (alert.title === "new-host") {
      // notificar o jogador que se tornou o novo Host
      this.roomHost.next(alert.data);
    }
    else if (alert.title === "finding-match") {
      this.findingMatch.next(alert.data === 'true');
    }
    else if (alert.title && alert.title === "new-room-id") {
      localStorage.setItem("lr", alert.data);
      this.lobbyRoomId.next(alert.data);
      console.log("LobbyId: ", alert.data)
      this.sessionService.observeSessionPlayer().subscribe(
        (player: any) => {
          if (player) this.addPlayerToArray(player.username);
        }
      );
    } 
    else if (alert.title && alert.title === "game-started") {
      this.gameRoomId.next(alert.data);
    }
    else console.log("[LobbyCommunication] Uknown Lobby Alert")
  }

  private setPlayersArray() {
    if (this.matchmaking) {
      this.players = [ 
        { in: false, data: undefined }, { in: false, data: undefined}
      ];
      this.roomPlayers = new BehaviorSubject<any[]>(this.players);
    } else {
      this.players = [
        { in: false, data: undefined }, { in: false, data: undefined}, 
        { in: false, data: undefined}, { in: false, data: undefined}
      ];
      this.roomPlayers = new BehaviorSubject<any[]>(this.players);
    }
  }

  private addPlayerToArray(data: any) {
    let i = 0, found = false;
    while (!found && i < this.players.length) { // verificar se o jogador já existe dentro da lista
      if (this.players[i].data && this.players[i].data === data) found = true;
      else i++;
    }
    i = 0; // se o jogador não existe na lista, insere na primeira posição vazia
    if (!found) while (i < this.players.length && this.players[i].in) {
      i++; // encontrar a primeira posição vazia
    }
    if (i < this.players.length) { this.players[i].in = true; this.players[i].data = data; }
    this.roomPlayers.next(this.players);
  }

  private removePlayerFromArray(data: any) {
    let i = 0, found = false;
    while (!found && i < this.players.length) { // encontrar o jogador dentro da lista
      if (this.players[i].data && this.players[i].data === data) found = true;
      else i++;
    }
    if (found) { this.players[i].in = false; this.players[i].data = undefined; }
    this.roomPlayers.next(this.players);
  }
}
