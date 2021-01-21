import { Component, Input, OnInit } from '@angular/core';

import { MatchHistoryService, MatchInfo } from 'src/app/services/api/match-history.service';

@Component({
  selector: 'app-match-history',
  templateUrl: './match-history.component.html',
  styleUrls: ['./match-history.component.css']
})
export class MatchHistoryComponent implements OnInit {

  matches: MatchInfo[] = [];
  @Input() player: any;
  awaitingResponse: boolean = false;

  constructor(private matchHistoryService: MatchHistoryService) { }

  ngOnInit(): void {
    this.awaitingResponse = true;
    if (this.player) {
      this.matchHistoryService.getPlayerMatches(this.player).subscribe(
        (list: any) => {
          if (list) this.matches = list;
          this.awaitingResponse = false;
        }
      )
    } else {
      this.matchHistoryService.getSessionPlayerMatches().subscribe(
        (list: any) => {
          if (list) this.matches = list;
          this.awaitingResponse = false;
        }
      )
    }
  }
}
