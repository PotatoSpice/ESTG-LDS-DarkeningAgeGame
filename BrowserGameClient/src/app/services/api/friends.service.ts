import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from "@angular/common/http";

import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

const API_ENDPOINT = environment.apiUrl;
const httpOptions = {
  headers: new HttpHeaders({
    "Content-Type": "application/json",
  }),
  withCredentials: true
};

export class FriendInfo {
  public username?: string;
  public online?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class FriendsService {

  private friendsList: BehaviorSubject<any>;

  constructor(private http: HttpClient) {
    this.friendsList = new BehaviorSubject<any>(undefined);
    this.refreshFriendsList();
  }

  public getFriendsList(): Observable<any> {
    return this.friendsList.asObservable();
  }

  public refreshFriendsList(): Observable<any> {
    return this.http.get(
      `${ API_ENDPOINT }/friends`, httpOptions
    ).pipe(
      tap( (list: any) => {
        if (list) this.friendsList.next(list);
      }),
      catchError( (error: any) => {
        if (error.status === 500) {
          console.log("[FriendsList] Something went wrong with the server ...")
        }
        else if (error.status === 401) {
          console.log("[FriendsList] Player is not signed in ...")
        }
        else console.log("[FriendsList] unexpected error: ", error);
        return of(false);
      })
    )
  }

  public sendFriendRequest(playerId: string): Observable<any> {
    return this.http.post(
      `${ API_ENDPOINT }/friends/send-invite`, { targetUsername: playerId }, httpOptions
    ).pipe(
      catchError( (error: any) => {
        let errMessage: string = '';
        if (error.status === 500) {
          errMessage = "Something went wrong with the server ..."
        }
        else if (error.status === 401) {
          errMessage = "Player is not signed in ..."
        }
        else if (error.status === 404) {
          errMessage = `Player not found ...`
        }
        else if (error.status === 400) {
          let response = error.error;
          if (response.type === "AlreadyRequested") {
            errMessage = `A Request has already been sent ...`
          } else if (response.type === "SendYourself") {
            errMessage = `A Player can't become friends with itself ...`
          } else if (response.type === "AlreadyFriends") {
            errMessage = `Players are friends already ...`
          } else {
            errMessage = "It seems you did something unexpected ...";
          }
        }
        else {
          errMessage = "unexpected error: ", error;
        }
        console.log("[FriendInvites] ", errMessage)
        return of(errMessage);
      })
    )
  }

  public acceptFriendRequest(requester: string, accept: boolean): Observable<any> {
    return this.http.post(
      `${ API_ENDPOINT }/friends/accept-invite`, { username: requester, response: accept }, httpOptions
    ).pipe(catchError( (error: any) => {
      if (error.status === 500) {
        console.log("[FriendNotifications] Something went wrong with the server ...")
      }
      else if (error.status === 401) {
        console.log("[FriendNotifications] Player is not signed in ...")
      }
      else if (error.status === 404) {
        console.log(`[FriendNotifications] Player not found for given friend request ...`)
      }
      else if (error.status === 400) {
        console.log(`[FriendNotifications] Friend Request not found ...`)
      }
      else console.log("[FriendNotifications] unexpected error: ", error);
      return of(false);
    }))
  }

  public removeFriend(playerId: string): Observable<any> {
    return this.http.delete(
      `${ API_ENDPOINT }/friends/unfriend/${ playerId }`, httpOptions
    ).pipe(
      catchError( (error: any) => {
        if (error.status === 500) {
          console.log("[FriendRemove] Something went wrong with the server ...")
        }
        else if (error.status === 401) {
          console.log("[FriendRemove] Player is not signed in ...")
        }
        else if (error.status === 404) {
          console.log(`[FriendRemove] Player not found ...`)
        }
        else if (error.status === 400) {
          let response = error.error;
          if (response.type === "FriendNotFound") {
            console.log(`[FriendRemove] Player and removing player are not friends ...`)
          } 
          else console.log("[FriendRemove] It seems you did something unexpected ...");
        }
        else console.log("[FriendRemove] unexpected error: ", error);
        return of(false);
      })
    )
  }
}
