import { Component, OnInit, Input } from '@angular/core';
import { AdminService } from '../admin.service';
import { Group } from './group';

@Component({
  selector: '.group',
  templateUrl: 'room-group.component.html',
  styleUrls: ['room-group.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupComponent implements OnInit {
  @Input() group: Group;

  constructor( private adminService: AdminService) {
  }

  ngOnInit() {
    console.log("RoomGroupComponent inited", this)
  }
}
