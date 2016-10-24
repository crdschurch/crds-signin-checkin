import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { Group } from './group';

@Component({
  selector: '.groupy',
  templateUrl: 'room-group.component.html',
  styleUrls: ['room-group.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupComponent implements OnInit {
  group: any;

  constructor( private adminService: AdminService) {
  }

  ngOnInit() {
    console.log("RoomGroupComponent inited", this)
  }
}
