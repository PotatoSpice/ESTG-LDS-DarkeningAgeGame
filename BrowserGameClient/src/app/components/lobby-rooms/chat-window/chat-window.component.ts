import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';

import { ChatMessage } from 'src/app/models/ChatMessage';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.css']
})
export class ChatWindowComponent implements OnInit {

  @Input() messages: ChatMessage[];
  messageForm = new FormControl('');
  @Output() message = new EventEmitter<string>();

  constructor() {
    this.messages = [];
  }

  ngOnInit(): void {
  }

  sendMessage() {
    this.message.emit(this.messageForm.value);
    this.messageForm.setValue('');
  }

}
