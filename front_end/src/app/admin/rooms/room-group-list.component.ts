import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdminService } from '../admin.service';
import { Group } from './group';
import { Room } from './room';
import { Event } from '../events/event';

@Component({
  templateUrl: 'room-group-list.component.html',
  styleUrls: ['room-group-list.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupListComponent implements OnInit {
  groups: Group[];
  eventId: string;
  roomId: string;
  private room: Room;
  private event: Event;

  constructor( private adminService: AdminService, private route: ActivatedRoute) {
  }

  private getData(): void {
    this.eventId = this.route.snapshot.params['eventId'];
    this.roomId = this.route.snapshot.params['roomId'];
    this.adminService.getRoomGroups(this.eventId, this.roomId).subscribe(
      groups => {this.groups = groups;},
      error => console.error(error)
    );
    this.adminService.getEvent(this.eventId).subscribe(
      event => { this.event = event; },
      error => console.error(error)
    );
  }

  getEvent(): Event {
    return this.isReady() ? this.event : new Event();
  }

  getGroups(): Group[] {
    return this.isReady() ? this.room.AssignedGroups : [];
  }

  getRoom(): Room {
    return this.isReady() ? this.room : new Room();
  }

  isReady(): boolean {
    return this.event !== undefined && this.room !== undefined;
  }

  ngOnInit() {
    this.getData();
  }
}
