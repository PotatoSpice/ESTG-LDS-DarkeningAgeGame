import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SessionService } from 'src/app/services/api/session.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {

  constructor(public sessionService: SessionService, public router: Router) { }

  ngOnInit(): void {
    this.sessionService.observeSessionPlayer().subscribe(
      (userInSession) => {
        if (userInSession) { // # redirect to session page if a user is in session
          this.router.navigate(['/session/hub'])
        }
      }
    )
  }

}
