import { Component, OnInit } from '@angular/core';
import { SessionService } from 'src/app/services/api/session.service';
import { Router } from '@angular/router';

export class SignUpPlayer {
  public username?: string;
  public email?: string;
  public firstName?: string;
  public lastName?: string;
  public password?: string;
  public confirmPassword?: string;
  public birthDate?: string;
}

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {

  player = new SignUpPlayer();
  awaitingResponse: boolean = false;
  warnings?: string;

  constructor(public sessionService: SessionService, public router: Router) { }

  ngOnInit(): void { }

  signUp(form: any): void {
    this.warnings = undefined;
    this.awaitingResponse = true;
    this.sessionService.signUp(form.value)
    .subscribe(
      () => {
        this.router.navigate(['/sign-in'])
      },
      (error: any) => {
        this.awaitingResponse = false;
        if (error.status === 500) {
          this.warnings = "Something went wrong with the server ..."
        } else if (error.status === 404) {
          let response = error.error;
          if (response.type === "EmailExists") {
            this.warnings = "Email is already in use."; 
          } else if (response.type === "UsernameExists") {
            this.warnings = "Username already exists."; 
          } else {
            this.warnings = "It seems you did something unexpected ..."; 
          }
        } else if (error.status === 400) {
          this.warnings = "Something unexpected happened while validating the data ..."; 
        }
        console.log(error);
      }
    )
  }

  returnBackButton() {
    this.router.navigate(['']);
  }

  redirectSignIn() {
    this.router.navigate(['sign-in']);
  }

}
