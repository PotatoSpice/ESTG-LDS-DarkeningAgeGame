import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

declare var $:any;

export interface ToastMessage {
  header: string;
  body: string;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  
  // Bootstrap 4 Toasts poderá ser substituido por esta biblioteca
  // https://github.com/CodeSeven/toastr 
  toastDataObs: BehaviorSubject<ToastMessage>;

  constructor() { 
    this.toastDataObs = new BehaviorSubject<ToastMessage>(
      {header: "Header", body: "body data"}
    );
  }

  public getCurrentToastData(): ToastMessage {
    return this.toastDataObs.value;
  }

  public observeToastData(): Observable<ToastMessage> {
    return this.toastDataObs.asObservable();
  }

  public showToast(header: string, body: string) {
    this.toastDataObs.next({header: header, body: body})
    $('.toast').toast('show');
  }
}
