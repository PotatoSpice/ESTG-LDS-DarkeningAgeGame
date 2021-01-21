import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { SessionService } from 'src/app/services/api/session.service';

export class ResetPwdRequest {
  public password?: string;
  public confirmPassword?: string;
}

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {

  resetData = new ResetPwdRequest();
  awaitingResponse: boolean = false;
  token: string = '';
  alerts?: string;

  constructor(public sessionService: SessionService, public router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    // check if has token param
    // send request to API after form verification
    // # User pode chegar ao /sign-in pelo redirect ou quando a sessão termina
    this.route.queryParams.subscribe((params) => { 
        if (params.token) {
          this.token = params.token;
        } else {
          this.token = '';
          this.alerts = 'Missing the token for password reset ... You should check your email.'
        }
      }
    )
  }

  resetPassword(form: any): void {
    if (this.token === '') {
      this.alerts = "Provided token is invalid ...";
    } else {
      this.alerts = undefined;
      this.awaitingResponse = true;
      this.sessionService.resetPassword(this.token, form.value.password, form.value.confirmPassword)
      .subscribe(
        () => {
          this.awaitingResponse = false;
          const options = { queryParams: { 'reset': true } }
          this.router.navigate(['sign-in'], options);
        },
        (error: any) => {
          this.awaitingResponse = false;
          if (error.status >= 500 && error.status <= 599) 
            this.alerts = "Something went wrong with the server ...";
          else if (error.status === 400) 
            this.alerts = "Something unexpected happened while validating the data ..."; 
          else if (error.status === 404) 
            this.alerts = "Provided token is invalid ...";
          else console.log(error)
        })
    }
  }

  returnBackButton() {
    this.router.navigate(['sign-in']);
  }

}
