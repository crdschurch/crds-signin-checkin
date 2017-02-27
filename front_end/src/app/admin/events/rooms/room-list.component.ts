
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute, Router, CanDeactivate } from '@angular/router';


import { Event, Room, Group } from '../../../shared/models';
import { AdminService } from '../../admin.service';
import { ApiService } from '../../../shared/services';
import { HeaderService } from '../../header/header.service';
import { RootService } from '../../../shared/services/root.service';

import * as moment from 'moment';
import * as _ from 'lodash';

import { CanDeactivateGuard } from '../../../shared/guards';

@Component({
  templateUrl: 'room-list.component.html',
  styleUrls: ['room-list.component.scss'],
  providers: [ CanDeactivateGuard ]
})

export class RoomListComponent implements OnInit {
  rooms: Room[];
  event: Event = null;
  eventId: string;
  isDirty = false;
  unassignedGroups: Group[];
  public dropdownStatus: { isOpen: boolean, isDisabled: boolean } = { isOpen: false, isDisabled: false };
  public isCollapsed = true;
  public hideClosedRooms = false;
  public closedRoomsLabelText = 'Hide Unused Rooms';

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private apiService: ApiService,
    private headerService: HeaderService,
    private router: Router,
    private rootService: RootService) {}

  private getData(): void {
    this.eventId = this.route.snapshot.params['eventId'];

    this.adminService.getRooms(this.eventId).subscribe(
      (rooms: Room[]) => {
        this.rooms = rooms;
      },
      (error: any) => console.error(error)
    );

    this.apiService.getEvent(this.eventId).subscribe(
      event => {
        this.event = event;
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );

    this.adminService.getUnassignedGroups(+this.eventId).subscribe(
      groups => {
        this.unassignedGroups = groups;
      },
      error => console.error(error)
    );
  }

  ngOnInit(): void {
    this.getData();
  }

  onNotifyDirty(message) {
    this.isDirty = message;
  }

  canDeactivate() {
    if (this.isDirty) {
      let c = confirm('You have unsaved changes. Are you sure you want to leave this page?');
      if (c) {
          return true;
      } else {
        return false;
      }
    } else {
      return true;
    }

  }

  public isReady(): boolean {
    return this.event !== undefined && this.rooms !== undefined;
  }

  public getOpenRooms() {
    return this.rooms ? _.filter(this.rooms, {'AllowSignIn': true}).length : ' ';
  }

  public getTotalRooms() {
    return this.rooms ? this.rooms.length : ' ';
  }

  public goToImport(): void {
    this.goToImportOrReset('import');
  }

  public goToReset(): void {
    this.goToImportOrReset('reset');
  }

  private goToImportOrReset(target: string): void {
    if (this.event && moment(this.event.EventStartDate).isBefore(moment())) {
      this.rootService.announceEvent('echeckCannotOverwritePastEvent');
      return;
    }

    this.router.navigate([`/admin/events/${this.eventId}/${target}`]);
  }

  public toggleDropdown($event: MouseEvent): void {
    $event.preventDefault();
    $event.stopPropagation();
    this.dropdownStatus.isOpen = !this.dropdownStatus.isOpen;
  }

  public toggleUnusedRooms(): void {
    debugger;
    if (this.hideClosedRooms === true) {
      debugger;
      this.hideClosedRooms = false;
      this.closedRoomsLabelText = 'Hide Unused Rooms'
    } else {
      debugger;
      this.hideClosedRooms = true;
      this.closedRoomsLabelText = 'Show Unused Rooms';
    }
  }

}
