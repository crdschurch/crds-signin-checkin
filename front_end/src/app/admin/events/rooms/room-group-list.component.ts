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
  private _selectedBumpingType: number;
  allAlternateRooms: Room[];
  isAdventureClub = false;
  alternateRoomsSelected = false;
  updating = false;
  isDirty = false;

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
      },
      error => {
        console.error(error);
      }
    );

    this.adminService.getRoomGroups(this.eventId, this.roomId).subscribe(
      room => {
        this.room = room;
        this.setCurrentEvent(room.AdventureClub);
        this.updating = false;
      },
      error => console.error(error)
    );

    this.apiService.getEvent(this.eventId).subscribe(

      event => {
        this.event = event;
        this.headerService.announceEvent(event);
      },
      error =>  {
        console.error(error);
      }
    );

    this.adminService.getBumpingRooms(this.route.snapshot.params['eventId'], this.route.snapshot.params['roomId']).subscribe(
      allRooms => {
        this.allAlternateRooms = Room.fromJsons(allRooms);
      },
      error => {
        console.error(error);
      }
    );

  }

  // get the adventure club event if AdventureClub is true for room, else return service event
  setCurrentEvent(isAdventureClub: boolean) {
    this.isAdventureClub = isAdventureClub;
    if (this.isAdventureClub) {
      this.eventToUpdate = _.filter(this.eventsMap, {'EventTypeId': Event.TYPE.ADVENTURE_CLUB})[0];
    } else {
      this.eventToUpdate = _.filter(this.eventsMap, {'ParentEventId': null})[0];
    }
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
        if (group.Selected || _.some(group.Ranges, { 'Selected': true})) {
          return true;
        }
    }
    return false;
  }

  hasBumpingRooms(): boolean {
    return this.allAlternateRooms
      .filter(
        (obj: Room) => { return obj.isBumpingRoom();
      }).length > 0;
  }

  getGroups(): Group[] {
    return this.isReady() ? this.room.AssignedGroups : [];
  }

  getRoom(): Room {
    return this.isReady() ? this.room : new Room();
  }

  isReady(): boolean {
    return (!this.updating && (this.event !== undefined && this.room !== undefined));
  }

  openTabIfAlternateRoomsHash() {
    if (this.route.snapshot.queryParams['tab'] === 'alternate-rooms') {
      this.alternateRoomsSelected = true;
    }
  }

  toggleAdventureClub(e) {
    if (this.isDirty) {
      this.isAdventureClub = !this.isAdventureClub;
      e.target.checked = this.isAdventureClub;
      this.rootService.announceEvent('saveBeforeAdventureClubToggle');
    } else if (this.hasActiveRooms() || this.hasBumpingRooms()) {
      this.isAdventureClub = !this.isAdventureClub;
      e.target.checked = this.isAdventureClub;
      if (this.hasActiveRooms()) {
        this.rootService.announceEvent('echeckAdventureClubToggleWarning');
      } else {
        this.rootService.announceEvent('removeBumpingRoomsBeforeAdventureClubToggle');
      }
    } else {
      this.setCurrentEvent(e.target.checked);
      this.setDirty();
    }
  }

  alternateRoomsSelect() {
    this.alternateRoomsSelected = true;
  }

  roomGroupsSelect() {
    this.alternateRoomsSelected = false;
  }

  ngOnInit() {
    this.getData();
    this.openTabIfAlternateRoomsHash();
  }

  setDirty() {
    this.isDirty = true;
  }

  setBumpingType(p) {
    this._selectedBumpingType = p;
  }

  save() {
    if (this.alternateRoomsSelected) {
      this.saveAlternateRooms();
    } else {
      this.saveRoomGroups();
    }
  }

  cancel() {
    this.updating = true;
    this.isDirty = false;
    this.getData();
  }

  saveRoomGroups() {
    this.updating = true;
    this.isDirty = false;

    this.adminService.updateRoomGroups(this.eventToUpdate.EventId.toString(), this.roomId, this.room).subscribe((resp) => {
      this.updating = false;
      this.rootService.announceEvent('roomGroupsUpdateSuccess');
      this.isDirty = false;
    }, error => (this.rootService.announceEvent('generalError')));
  }

  saveAlternateRooms() {
    // dont allow saving of bumping rules on AC rooms as it creates bad data
    if (this.isAdventureClub) {
      return this.rootService.announceEvent('echeckNoACBumpingRules');
    }
    this.updating = true;
    this.isDirty = false;

    // set bumping type based on radio selection
    for (let room of this.allAlternateRooms) {
      room.BumpingRuleTypeId = this._selectedBumpingType;
    }

    this.adminService.updateBumpingRooms(this.route.snapshot.params['eventId'],
      this.route.snapshot.params['roomId'], this.allAlternateRooms).subscribe((resp) => {
        this.updating = false;
        this.rootService.announceEvent('alternateRoomsUpdateSuccess');
        this.isDirty = false;
      }, error => (this.rootService.announceEvent('generalError')));
    }

}
