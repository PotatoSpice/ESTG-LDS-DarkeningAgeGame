import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  player: string = '';

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    const temp = this.route.snapshot.paramMap.get('userId');
    this.player = temp ? temp : '';
  }

}
