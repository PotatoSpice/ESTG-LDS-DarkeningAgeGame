import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { interval, Subscription } from 'rxjs';
import { delay, repeat, switchMap } from 'rxjs/operators';

import { SessionService } from '../../../services/api/session.service';
import { LobbyCommunicationService } from '../../../services/sockets/lobby-communication.service';
import { GameRoomCommunicationService } from '../../../services/sockets/game-room-communication.service';
import { ToastService } from 'src/app/services/toast.service';

import { ChatMessage } from 'src/app/models/ChatMessage';
import { ErrorMessage } from 'src/app/models/ErrorMessage';
import { GameUpdatedConn } from 'src/app/models/GameUpdatedConn';

@Component({
  selector: 'app-room-game-starting',
  templateUrl: './room-game-starting.component.html',
  styleUrls: ['./room-game-starting.component.css']
})
export class RoomGameStartingComponent implements OnInit {
  // connection flags
  connected: boolean = false;
  gameStarted: boolean = false;
  connError: boolean = false;
  // chat messages and player cards lists
  messages: ChatMessage[] = [];
  players: any[] = [];
  // player and room identifications
  playerId: string = "";
  playerToken: string = "";
  gameRoomId: string = "";
  // timer veriables
  private subscription?: Subscription;
  private timeLeft = 40; // seconds
  public time?: number;

  constructor(private gameService: GameRoomCommunicationService, private lobbyService: LobbyCommunicationService,
    private sessionService: SessionService, private toastService: ToastService, private router: Router) {
      // window.addEventListener("beforeunload", (event) => {
      //   event.preventDefault();
      //   event.returnValue = `Your connection to the server will be closed. 
      //     Don't worry, you can still reconnect to the game if you leave right now.`;
      //   return event;
      //   // use return to prompt user
      // });
    }

  ngOnInit(): void {
    // This page should only connect to the server only when needed.
    if (history.state.action && history.state.action === 'redirect') {
      // Observe changes to the player in session
      this.sessionService.observeSessionPlayer().subscribe(
        (player: any) => {
          const temp = localStorage.getItem('auth-token');
          if (player && player !== '' && temp) {
            this.playerId = player.username;
            this.playerToken = temp;
          }
          else this.playerId = this.playerToken = '';
        }
      );
      // Observe new chat messages
      this.gameService.observeChatMessages().subscribe(
        (message: ChatMessage | null) => {
          if (message) this.messages.push(message);
        }
      );
      // Observe players in room
      this.gameService.observeRoomPlayers().subscribe(
        (players: any[]) => this.players = players
      );
      this.lobbyService.observeGameRoomId().subscribe(
        (room: string) => this.gameRoomId = room
      );

      this.lobbyService.checkIfInRoom().pipe(
        delay(1000),
        switchMap( room => {
          console.log(room)
          if (room && room !== '') {
            this.connected = true;
            this.gameRoomId = room;
          } else {
            this.connected = false;
            this.gameRoomId = '';
          }
          let token = localStorage.getItem('auth-token');
          return this.gameService.connectToGameRoom(this.gameRoomId, token ? token : '');
        })
      ).subscribe(
        (msg: any) => {
          if (msg instanceof ErrorMessage) {
            this.toastService.showToast(msg.type, msg.message);
            // THIS PAGE SHOULD ONLY BE ACCESSIBLE ONCE, BEFORE THE GAME STARTS
            if (msg.type === 'PlayerGameStarted') {
              this.router.navigate(['session', 'reconnect']);
            } 
            else if (msg.type === 'Something went wrong!') {
              this.router.navigate(['session', 'hub']);
            }
          }
          else if (msg instanceof GameUpdatedConn) {
            this.connected = true;
            this.subscription = interval(1000).subscribe(x => { this.getTimeDifference(); });
          }
          else if (msg instanceof ChatMessage) {
            // placeholder
          }
          else if (msg instanceof GameUpdatedConn) {
            // placeholder
          }
        },
        (err: any) => {
          this.connError = this.connected = false;
          console.error(err)
        },
        () => this.disconnect()
      );
    }
    else this.router.navigate(['/session/hub']);
  }

  onMessage(message: string) {
    if (this.connected) {
      this.gameService.sendChatMessage(message);
    } else {
      alert("Not connected!")
    }
  }

  private disconnect() {
    this.gameService.leaveBrowser();
    console.log('left')
    this.connError = this.connected = false;
    this.messages = [];
  }

  private getTimeDifference() {
    if (this.timeLeft >= 0) {
      this.updateClock(this.timeLeft);
      this.timeLeft = this.timeLeft - 1;
    } else {
      window.open(`DarkeningAgeGame:room-id=${this.gameRoomId}&player-id=${this.playerId}&player-auth=${this.playerToken}`, '_self');
      this.router.navigate(['session', 'reconnect']);
    }
  }

  private updateClock(timeDiffSeconds: number) {
    this.time = Math.floor((timeDiffSeconds) % 60);
  }

  ngOnDestroy(): void {
    this.disconnect();
    this.subscription?.unsubscribe();
  }

}
