import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from "@angular/common/http";

import { environment } from 'src/environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

const API_ENDPOINT = environment.apiUrl;
const httpOptions = {
  headers: new HttpHeaders({
    "Content-Type": "application/json",
  }),
  withCredentials: true
};

export class MatchInfo {
  public playerId?: string;
  public gameID?: string;
  public placement?: number;
  public armiesCreated?: number;
  public regionsConquered?: number;
  public date?: string;
}

@Injectable({
  providedIn: 'root'
})
export class MatchHistoryService {

  constructor(private http: HttpClient) { }

  public getPlayerMatches(player: string): Observable<any> {
    return this.http.get(
      `${ API_ENDPOINT }/match-history/player/${ player }?nview=10`, httpOptions
    ).pipe(
      map( (info: any): any => {
        const matches: MatchInfo[] = info;
        matches.map( (info) => {
          if (info.date) {
            info.date = new Date(Date.parse(info.date)).toUTCString();
          }
        })
        return info;
      }),
      catchError( (error: any) => {
        if (error.status === 500) {
          console.log("[PlayerMatchHistory] Something went wrong with the server ...")
        }
        else if (error.status === 401) {
          console.log("[PlayerMatchHistory] Player is not signed in ...")
        }
        else if (error.status === 404) {
          let response = error.error;
          if (response.type === "PlayerNotFound") {
            console.log(`[PlayerMatchHistory] Player not found ...`)
          } else console.log(`[PlayerMatchHistory] It seems you did something unexpected ...`)
        }
        else console.log("[PlayerMatchHistory] unexpected error: ", error);
        return of(false);
      })
    )
  }

  public getSessionPlayerMatches(): Observable<any> {
    return this.http.get(
      `${ API_ENDPOINT }/match-history`, httpOptions
    ).pipe(
      map( (info: any): any => {
        const matches: MatchInfo[] = info;
        matches.map( (info) => {
          if (info.date) {
            info.date = new Date(Date.parse(info.date)).toUTCString();
          }
        })
        return info;
      }),
      catchError( (error: any) => {
        if (error.status === 500) {
          console.log("[PlayerMatchHistory] Something went wrong with the server ...")
        }
        else if (error.status === 401) {
          console.log("[PlayerMatchHistory] Player is not signed in ...")
        }
        else console.log("[PlayerMatchHistory] unexpected error: ", error);
        return of(false);
      })
    )
  }
}
