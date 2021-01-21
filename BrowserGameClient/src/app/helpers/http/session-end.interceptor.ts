import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { SessionService } from '../../services/api/session.service';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';

@Injectable()
export class SessionEndInterceptor implements HttpInterceptor {

  constructor(public sessionService: SessionService, public router: Router) {}
  
  intercept( req: HttpRequest<any>, next: HttpHandler ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && req.headers.has('WWW-Authenticate')) {
          // 401 handled in auth.interceptor
          this.sessionService.clearSession();
        }
        return throwError(error);
      })
    );
  }
}
