import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class JwtTokenInterceptor implements HttpInterceptor {

  constructor() {}

  intercept( req: HttpRequest<any>, next: HttpHandler ): Observable<HttpEvent<any>> {
    const token = localStorage.getItem("auth-token")
    // add saved auth token to header for every request
    let modified = req;
    if (token) {
      modified = req.clone({ setHeaders: { "Authorization": `Bearer ${ token }` } });
    }
    return next.handle(modified);
  }
}
