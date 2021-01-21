import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from "@angular/common/http";

import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { share } from 'rxjs/operators';

const API_ENDPOINT = environment.apiUrl;
const httpOptions = {
  headers: new HttpHeaders({
    "Content-Type": "application/json",
  }),
  withCredentials: true
};

export class SessionPlayer {
  public username?: string;
  public email?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  // # Controlo de sessão do utilizador
  expired?: Boolean;

  private sessionPlayer: BehaviorSubject<any>;

  constructor(private http: HttpClient) { 
    // this.http.get(
    //   `${API_ENDPOINT}/session/session-user`, httpOptions).subscribe( (user) => {
    //     this.sessionPlayer = new BehaviorSubject<any>(user);
    //   });
    let temp = localStorage.getItem('session');
    this.sessionPlayer = new BehaviorSubject<any>(temp ? JSON.parse(temp) : null)
  }

  public getSessionPlayer(): any {
    return this.sessionPlayer.getValue();
  }

  public observeSessionPlayer(): Observable<any> {
    return this.sessionPlayer.asObservable();
  }

  public signIn(username: string, password: string): Observable<any> {
    const request = this.http.post(
      `${ API_ENDPOINT }/session/sign-in`, { username: username, password: password }, httpOptions)
      // share() used so multiple subscribes have the same response
      .pipe(share());

    // save the JWT here
    request.subscribe( (response: any) => {
      const { token, playerData } = response;
      this.sessionPlayer.next( playerData );
      localStorage.setItem("session", JSON.stringify( playerData ));
      localStorage.setItem("auth-token", token);
    });

    return request;
  }

  public signUp(userData: any): Observable<any> {
    return this.http.post(
      `${ API_ENDPOINT }/session/sign-up`, userData, httpOptions
    )
  }

  public forgotPassword(email: string): Observable<any> {
    return this.http.post(
      `${ API_ENDPOINT }/session/forgot-password`, {
        email: email
      }, httpOptions
    )
  }

  public resetPassword(token: string, newPassword: string, confirm: string): Observable<any> {
    return this.http.post(
      `${ API_ENDPOINT }/session/reset-password`, {
        token: token,
        password: newPassword,
        confirmPassword: confirm
      }, httpOptions
    )
  }

  /**
   * This should be called to clear session manually
   */
  public signOut() {
    this.expired = false;
    this.sessionPlayer.next(null);
    localStorage.removeItem("session");
    localStorage.removeItem("auth-token");
    this.http.post(`${ API_ENDPOINT }/session/sign-out`, {}, httpOptions)
  }

  /**
   * This should be called when JWT token expires
   */
  public clearSession() {
    this.expired = true;
    this.sessionPlayer.next(null);
    localStorage.removeItem("session");
    localStorage.removeItem("auth-token");
  }
}
