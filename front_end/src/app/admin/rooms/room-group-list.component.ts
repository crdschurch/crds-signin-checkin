import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdminService } from '../admin.service';
import { HeaderService } from '../header/header.service';
import { Group } from './group';
import { Room } from './room';
import { Event } from '../events/event';

@Component({
  templateUrl: 'room-group-list.component.html',
  styleUrls: ['room-group-list.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupListComponent implements OnInit {
  private room: Room;
  private event: Event;

  constructor( private adminService: AdminService, private route: ActivatedRoute, private headerService: HeaderService) {
  }

  private getData(): void {
    const eventId = this.route.snapshot.params['eventId'];
    const roomId = this.route.snapshot.params['roomId'];
    this.adminService.getRoomGroups(eventId, roomId).subscribe(
      room => { this.room = room; },
      error => console.error(error)
    );
    this.adminService.getEvent(eventId).subscribe(
      event => {
        this.event = event;
        this.headerService.announceEvent(event);
      },
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
