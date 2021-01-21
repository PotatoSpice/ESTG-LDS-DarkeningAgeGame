import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from "@angular/common/http";

import { environment } from 'src/environments/environment';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

const API_ENDPOINT = environment.apiUrl;
const httpOptions = {
  headers: new HttpHeaders({
    "Content-Type": "application/json",
  }),
  withCredentials: true
};

export class GameInvite {
  public roomId?: string;
  public hostId?: string;
  public gameType?: string;
  public createDate?: string;
}

export class SendGameInvite {
  public roomId: string;
  public invitedId: string;
  public gameType: string;

  constructor (p_roomId: string, p_invitedId: string, p_gameType: string) {
    this.roomId = p_roomId;
    this.invitedId = p_invitedId;
    this.gameType = p_gameType;
  }
}

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  constructor(private http: HttpClient) { }

  public getGameNotifications(): Observable<any> {
    return this.http.get(
      `${ API_ENDPOINT }/game-invites`, httpOptions
    )
  }

  public getFriendRequests(): Observable<any> {
    return this.http.get(
      `${ API_ENDPOINT }/friends/invites`, httpOptions
    )
  }

  public sendGameInvitation(invitationData: SendGameInvite): Observable<any> {
    return this.http.post(
      `${ API_ENDPOINT }/game-invites/send`, invitationData, httpOptions
    ).pipe(
      catchError( (error: any) => {
        if (error.status === 500) {
          console.log("[LobbyInvites] Something went wrong with the server ...")
        }
        else if (error.status === 401) {
          console.log("[LobbyInvites] Player is not signed in ...")
        }
        else if (error.status === 404) {
          console.log(`[LobbyInvites] Player not found ...`)
        }
        else if (error.status === 400) {
          let response = error.error;
          if (response.type === "AlreadyRequested") {
            console.log(`[LobbyInvites] The Player has been already invited ...`)
          } else if (response.type === "SendYourself") {
            console.log(`[LobbyInvites] A Player can't invite itself ...`)
          } else if (response.type === "FriendNotFound") {
            console.log(`[LobbyInvites] The Inviter should be friends with the invitee ...`)
          } 
          else console.log("[LobbyInvites] It seems you did something unexpected ...");
        }
        else console.log("[LobbyInvites] unexpected error: ", error);
        return of(false);
      })
    )
  }

  public revokeGameInvitation(roomId: string): Observable<any> {
    return this.http.delete(
      `${ API_ENDPOINT }/game-invites/delete/${ roomId }`, httpOptions
    ).pipe(
      catchError( (error: any) => {
        if (error.status === 500) {
          console.log("[GameNotifications] Something went wrong with the server ...")
        }
        else if (error.status === 401) {
          console.log("[GameNotifications] Player is not signed in ...")
        }
        else if (error.status === 404) {
          console.log(`[GameNotifications] Invite not found for given room ${roomId} ...`)
        }
        else console.log("[GameNotifications] unexpected error: ", error);
        return of(false);
      })
    )
  }
}
