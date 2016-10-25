import { Component, Input } from '@angular/core';
import { AdminService } from '../admin.service';
import { Group } from './group';

@Component({
  selector: '.group',
  templateUrl: 'room-group.component.html',
  styleUrls: ['room-group.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupComponent {
  @Input() group: Group;

  constructor( private adminService: AdminService) {
  }

  toggleAll(group) {
    console.log(`toggle all`, group)
  }

  toggleRange(range) {
    console.log(`toggle range`, range)
  }

}
