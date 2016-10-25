import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdminService } from '../admin.service';
import { Group } from './group';

@Component({
  templateUrl: 'room-group-list.component.html',
  styleUrls: ['room-group-list.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupListComponent implements OnInit {
  groups: Group[];
  eventId: string;
  roomId: string;

  constructor( private adminService: AdminService, private route: ActivatedRoute) {
  }

  private getData(): void {
    this.eventId = this.route.snapshot.params['eventId'];
    this.roomId = this.route.snapshot.params['roomId'];
    this.adminService.getRoomGroups(this.eventId, this.roomId).subscribe(
      groups => {this.groups = groups;},
      error => console.error(error)
    );
  }

  ngOnInit() {
    this.getData()
  }
}
