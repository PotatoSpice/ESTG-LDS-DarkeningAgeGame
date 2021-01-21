import { Component, Input, OnInit } from '@angular/core';
import { ToastMessage, ToastService } from 'src/app/services/toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.css']
})
export class ToastComponent implements OnInit {

  toast_data!: ToastMessage;

  constructor(private toastService: ToastService) { }

  ngOnInit(): void {
    this.toastService.observeToastData().subscribe(
      (toast: ToastMessage) => {
        this.toast_data = toast;
      }
    )
  }

}
