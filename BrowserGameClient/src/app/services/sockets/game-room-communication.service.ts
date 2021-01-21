import { Injectable } from '@angular/core';

import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { SocketConnectionService } from './socket-connection.service';
import { SessionService } from '../api/session.service';

import { ChatMessage } from '../../models/ChatMessage';
import { GameUpdatedConn } from '../../models/GameUpdatedConn';
import { ErrorMessage } from 'src/app/models/ErrorMessage';

const API_ENDPOINT = environment.gameUrl + "/ws-game";

@Injectable({
  providedIn: 'root'
})
export class GameRoomCommunicationService {

  private connected: boolean = false;

  private players: any[];
  private roomPlayers: BehaviorSubject<any[]>;
  private chatMessages: BehaviorSubject<ChatMessage | null>;

  constructor(private wsConnectionService: SocketConnectionService, private sessionService: SessionService) {
    this.players = [{}, {}, {}, {}];
    this.roomPlayers = new BehaviorSubject<any[]>(this.players);
    this.chatMessages = new BehaviorSubject<ChatMessage | null>(null);
  }

  public connectToGameRoom(roomId: string, player_token: string): Observable<any> {
    return this.wsConnectionService.connect(
      `${API_ENDPOINT}?room-id=${roomId}&player-auth=${player_token}`).pipe(
        map( (response: MessageEvent): any => {
          this.connected = true;
          const data = JSON.parse(response.data);
          console.log("[DEV - GameRoomResponse] ", data);

          if (data.EventType && data.EventType === "ChatMessage") {
            const message = new ChatMessage(data.Username, data.Message, '');
            this.chatMessages.next(message);
            return message;
            
          } else if (data.eventType && data.eventType === "GameUpdatedConn") {
            const playerList = new GameUpdatedConn(data.data);
            data.data.map( (value: any, index: any, array: any[]) => {
              this.players[index] = value;
            });
            this.roomPlayers.next(this.players);
            return playerList;
            
          } else if (data.errorType) {
            let errorTitle = "Something went wrong!", message = data.message;
            if (data.errorType === "GameWarningException") {
              errorTitle = 'Warning!';
            } 
            else if (data.errorType === "RoomNotFoundException") {
              errorTitle = 'Something went wrong!';
              message = "It seems you're not in a game yet. You should join a lobby first or wait for the Game Room to be loaded ...";
            }
            else if (data.errorType === "PlayerGameStarted") {
              errorTitle = 'PlayerGameStarted';
            }
            return new ErrorMessage(data.errorType, errorTitle, message);
      
          }
          else return response.data;
        }),
        catchError( (err: any) => {
          console.log('[GameServiceSocketError]', err);
          this.wsConnectionService.closeConnection();
          this.connected = false;
          return of('SocketError');
        })
      );
  }

  public sendChatMessage(message: string) {
    if (this.connected) this.wsConnectionService.sendMessage({ 
      eventType: "ChatMessage", 
      data: [this.sessionService.getSessionPlayer().username, message] });
  }

  public leaveBrowser() {
    if (this.connected && this.wsConnectionService.closeConnection()) {
      this.connected = false;
    }
  }

  public observeChatMessages(): Observable<ChatMessage | null> {
    return this.chatMessages.asObservable();
  }

  public observeRoomPlayers(): Observable<any[]> {
    return this.roomPlayers.asObservable();
  }
}
