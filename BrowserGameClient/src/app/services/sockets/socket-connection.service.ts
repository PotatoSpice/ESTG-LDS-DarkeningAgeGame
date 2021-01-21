import { Injectable } from '@angular/core';

import { Observable, Observer } from "rxjs";
import { webSocket, WebSocketSubject } from "rxjs/webSocket";
import { delay, retryWhen, share } from 'rxjs/operators'

@Injectable({
  providedIn: 'root'
})
export class SocketConnectionService {

  private webSocketConn?: WebSocket;
  
  constructor() { }

  /**
   * Opens a WebSocket connection and attaches it's events into an Observable.
   * 
   * Example use:
   * webSocketObserver.subscribe( 
   *  (msg) => console.log("[SocketEventProvider] received websocket message ..."),
   *  (err) => console.log("[SocketEventProvider] received websocket error ..."),
   *  () => console.log("[SocketEventProvider] socket closed ...")
   * );
   * 
   * @param url websocket server url
   * @returns an Observable for the websocket events (message, error, close)
   */
  public connect(url: string): Observable<MessageEvent> {
    if (!this.webSocketConn) {
      console.log("[Websocket] connected")
      this.webSocketConn = new WebSocket(url);
    }

    const webSocketObserver = new Observable(
      (observer: Observer<MessageEvent>) => {
        if (this.webSocketConn) {
          this.webSocketConn.onmessage = observer.next.bind(observer);
          this.webSocketConn.onerror = observer.error.bind(observer);
          this.webSocketConn.onclose = observer.complete.bind(observer);
        } else console.log("WebSocket undefined.")
        return this.webSocketConn?.close.bind(this.webSocketConn);
      }
    ).pipe(share());

    return webSocketObserver;
  }

  /**
   * @param data data to send through websocket. Should be a JSON object or array!
   */
  public sendMessage(data: any) {
    if (this.webSocketConn && this.webSocketConn.readyState === WebSocket.OPEN) {
      this.webSocketConn.send(JSON.stringify(data));
    }
  }

  /**
   * Close socket connection.
   */
  public closeConnection(): boolean {
    if (this.webSocketConn) {
      console.log("[Websocket] disconnected")
      this.webSocketConn.close();
      this.webSocketConn = undefined;
      return true;
    } else return false;
  }

  // METHODS BELOW ARE DEPRECATED ####################################################################################

  private socketHandler: WebSocketSubject<any> | undefined;
  
  /**
   * Start WebSocket conection with Rxjs 6 Wrapper.
   * 
   * Example use:
   * webSocketHandler.subscribe(
   *  (msg) => console.log('message received: ' + msg),
   *  (err) => console.log(err),
   *  () => console.log('complete')
   * );
   * 
   * @param url the url of the websocket server or default - {@constant API_ENDPOINT}
   * @deprecated this version uses Rxjs 6 wrapper around the webSocket.
   */
  public connectWithWrapper(url: string): Observable<MessageEvent> {
    if (!this.socketHandler) {
      console.log("CONNECTING TO WEBSOCKET SERVER")
      console.log("API_URL: " + url)
      this.socketHandler = webSocket( url );
      this.socketHandler.pipe( // retry the socket connection after 10 seconds
        retryWhen((errors) => errors.pipe(delay(10))));
    }
    return this.socketHandler.asObservable();
  }

  /**
   * @param data data to send through websocket. Should be a JSON object or array!
   * @deprecated this version uses Rxjs 6 wrapper around the webSocket.
   */
  public sendMessageWithWrapper(data: any) {
    if (this.socketHandler) {
      this.socketHandler.next(data);
    } else {
      // warning
    }
  }

  /**
   * Close the socket connection.
   * @deprecated this version uses Rxjs 6 wrapper around the webSocket.
   */
  public closeConnectionWithWrapper() {
    if (this.socketHandler) {
      this.socketHandler.complete();
      this.socketHandler = undefined;
      console.log('[DataService]: connection closed');
    } else {
      // warning
    }
  }
}
