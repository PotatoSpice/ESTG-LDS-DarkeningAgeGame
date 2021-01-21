import { Component, OnInit } from '@angular/core';
import { SessionService } from 'src/app/services/api/session.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl } from '@angular/forms';

export class SignInPlayer {
  public username?: string;
  public password?: string;
}

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

  player = new SignInPlayer();
  emailForm = new FormControl('');
  awaitingResponse: boolean = false;
  info?: string;
  warnings?: string;
  alerts?: string;

  constructor(public sessionService: SessionService, public router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    // # User pode chegar ao /sign-in pelo redirect ou quando a sessão termina
    this.route.queryParams.subscribe((params) => { 
        if (params.expired) {
          this.alerts = 'Your session has expired!'
        } else if (params.reset) {
          this.info = 'Your password has been reset successfully!'
        }
      }
    )
  }

  signIn(form: any): void {
    this.warnings = this.alerts = this.info = undefined;
    this.awaitingResponse = true;
    this.sessionService.signIn(form.value.username, form.value.password)
    .subscribe(
      () => {
        this.router.navigate(['/session/hub'])
      },
      (error) => {
        this.awaitingResponse = false;
        if (error.status >= 500 && error.status <= 599) {
          this.alerts = "Something went wrong with the server ...";
        } else if (error.status === 404 || error.status === 400) {
          this.warnings = "Something unexpected happened while fetching your data ..."; 
        } else if (error.status === 401) {
          this.warnings = "Invalid credentials. Check if username or password are correct!"; 
        }
        console.log(error)
      }
    )
  }

  forgotPassword() {
    if (this.emailForm.valid) {
      console.log(this.emailForm.value)
      this.sessionService.forgotPassword(this.emailForm.value).subscribe(
        (response: any) => {
          if (response) this.info = response.message;
        }
      );
    }
  }

  returnBackButton() {
    this.router.navigate(['']);
  }

  redirectSignUp() {
    this.router.navigate(['sign-up']);
  }
  
}
