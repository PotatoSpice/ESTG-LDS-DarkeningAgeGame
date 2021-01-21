import { Component, OnInit } from '@angular/core';

import { PlayersService } from './services/api/players.service';
import { SocketConnectionService } from './services/sockets/socket-connection.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'browserGameClient';

  listener: any;

  constructor(private wsConnectionService: SocketConnectionService, private playersService: PlayersService) {
    this.listener = (event: any) => {
      this.playersService.setPlayerStatus(false).subscribe();
      this.wsConnectionService.closeConnection();
      return undefined;
   }
  }

  ngOnInit() {
    window.addEventListener("beforeunload", this.listener);
  }

  ngOnDestroy() {
    // window.removeEventListener("beforeunload", this.listener);
  }
}
