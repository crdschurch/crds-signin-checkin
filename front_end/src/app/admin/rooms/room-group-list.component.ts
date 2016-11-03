import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { AdminService } from '../admin.service';
import { HeaderService } from '../header/header.service';
import { Group } from './group';
import { Room } from './room';
import { Event } from '../events/event';

@Component({
  templateUrl: 'room-group-list.component.html',
  styleUrls: ['room-group-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoomGroupListComponent implements OnInit {
  groups: Group[];
  eventId: string;
  roomId: string;
  private room: Room;
  private event: Event;
  alternateRoomsSelected: boolean = false;

  constructor( private adminService: AdminService,
               private route: ActivatedRoute,
               private headerService: HeaderService,
               private location: Location) {
  }

  private getData(): void {
    this.eventId = this.route.snapshot.params['eventId'];
    this.roomId = this.route.snapshot.params['roomId'];
    this.adminService.getRoomGroups(this.eventId, this.roomId).subscribe(
      room => {
        this.room = room;
      },
      error => console.error(error)
    );
    this.adminService.getEvent(this.eventId).subscribe(
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

  goBack() {
    this.location.back();
  }

  openTabIfAlternateRoomsHash() {
    if (this.route.snapshot.queryParams['tab'] === 'alternate-rooms') {
      this.alternateRoomsSelected = true;
    }
  }

  ngOnInit() {
    this.getData();
    this.openTabIfAlternateRoomsHash();
  }
}
