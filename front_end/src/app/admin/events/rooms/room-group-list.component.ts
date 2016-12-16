import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { HeaderService } from '../../header/header.service';
import { Group, Room, Event } from '../../../shared/models';
import { AdminService } from '../../admin.service';
import { ApiService, RootService } from '../../../shared/services';
import * as _ from 'lodash';

@Component({
  templateUrl: 'room-group-list.component.html',
  styleUrls: ['room-group-list.component.scss']
})
export class RoomGroupListComponent implements OnInit {
  groups: Group[];
  eventId: string;
  // this is the event we should sent to backend when updating groups
  // as this will either be the adventure club event or the service event (non AC)
  eventToUpdate: Event;
  roomId: string;
  private room: Room;
  private event: Event;
  private eventsMap: Event[];
  private isAdventureClub: boolean = false;
  alternateRoomsSelected: boolean = false;
  updating: boolean = false;

  constructor( private apiService: ApiService,
               private adminService: AdminService,
               private rootService: RootService,
               private route: ActivatedRoute,
               private headerService: HeaderService,
               private location: Location) {
  }

  private getData(): void {
    this.eventId = this.route.snapshot.params['eventId'];
    this.roomId = this.route.snapshot.params['roomId'];

    this.apiService.getEventMaps(this.eventId).subscribe(
      events => {
        this.eventsMap = events;
        console.log(this);
      },
      error => console.error(error)
    );

    this.adminService.getRoomGroups(this.eventId, this.roomId).subscribe(
      room => {
        this.room = room;
        this.isAdventureClub = this.room.AdventureClub;
        if (this.isAdventureClub) {
          this.eventToUpdate = _.filter(this.eventsMap, {'EventTypeId': 20})[0];
        } else {
          this.eventToUpdate = _.filter(this.eventsMap, {'ParentEventId': null})[0];
        }
      },
      error => console.error(error)
    );

    this.apiService.getEvent(this.eventId).subscribe(

      event => {
        this.event = event;
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );

  }

  public isUpdating(): boolean {
    return this.updating;
  }

  public setUpdating(updating: boolean): void {
    this.updating = updating;
  }

  getEvent(): Event {
    return this.isReady() ? this.event : new Event();
  }

  hasActiveRooms(): boolean {
    for (let group of this.room.AssignedGroups) {
        if (_.some(group.Ranges, { 'Selected': true})) {
          return true;
        }
    }
    return false;
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

  toggleAdventureClub(e) {
    // if has rooms, dont change toggle, show alert
    if (this.hasActiveRooms()) {
      this.isAdventureClub = !this.isAdventureClub;
      e.target.checked = this.isAdventureClub;
      this.rootService.announceEvent('echeckAdventureClubToggleWarning');
    } else {
      if (e.target.checked) {
        this.eventToUpdate = _.filter(this.eventsMap, {'EventTypeId': 20})[0];
      } else {
        this.eventToUpdate = _.filter(this.eventsMap, {'ParentEventId': null})[0];
      }
      console.log(this.eventToUpdate)
    }
  }

  ngOnInit() {
    this.getData();
    this.getData();
    this.openTabIfAlternateRoomsHash();
  }

}
