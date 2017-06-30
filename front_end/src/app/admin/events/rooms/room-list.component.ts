
import { AfterViewChecked, Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute, Router, CanDeactivate } from '@angular/router';

import { Event, Room, Group } from '../../../shared/models';
import { AdminService } from '../../admin.service';
import { ApiService } from '../../../shared/services';
import { RootService } from '../../../shared/services/root.service';

import * as moment from 'moment';
import * as _ from 'lodash';

import { CanDeactivateGuard } from '../../../shared/guards';

declare let jQuery;

@Component({
  templateUrl: 'room-list.component.html',
  styleUrls: ['room-list.component.scss'],
  providers: [ CanDeactivateGuard ]
})

export class RoomListComponent implements OnInit, AfterViewChecked {
  rooms: Room[];
  event: Event = null;
  eventId: string;
  isDirty = false;
  isSaving = false;
  unassignedGroups: Group[];
  public dropdownStatus: { isOpen: boolean, isDisabled: boolean } = { isOpen: false, isDisabled: false };
  public isCollapsed = true;
  public hideClosedRooms = false;

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private apiService: ApiService,
    private router: Router,
    private rootService: RootService) {}

  private getData(): void {
    this.eventId = this.route.snapshot.params['eventId'];

    this.adminService.getRooms(this.eventId).subscribe(
      (rooms: Room[]) => {
        // sort by KcSortOrder ascending, if null, put at end
        this.rooms = rooms.sort((r1, r2) => {
            return +(r1.KcSortOrder == null) - +(r2.KcSortOrder == null)
              || +(r1.KcSortOrder > r2.KcSortOrder)
              || -(r1.KcSortOrder < r2.KcSortOrder);
        });
      },
      (error: any) => console.error(error)
    );

    this.apiService.getEvent(this.eventId).subscribe(
      event => {
        this.event = event;
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

  onNotifySaving(message) {
    this.isSaving = message;
  }

  // update the allow checkin property on the matching room in the list component - this is necessary to allow the
  // allow checkin filter to properly hide and show closed rooms
  updateRooms(message) {
    this.rooms.find(r => r.RoomId === message.RoomId).AllowSignIn = message.AllowSignIn;
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

  isZeroCapacityAndOpen(room) {
    return room.AllowSignIn && !room.Capacity;
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

  public goToImportTemplate(): void {
    this.goToImportOrReset('import/templates');
  }

  public goToImport(): void {
    this.goToImportOrReset('import/events');
  }

  public goToReset(): void {
    this.goToImportOrReset('reset');
  }

  private goToImportOrReset(target: string): void {
    if (this.event && moment(this.event.EventStartDate).isBefore(moment())) {
      this.rootService.announceEvent('echeckCannotOverwritePastEvent');
      return;
    }
    this.router.navigate([`/admin/events/${this.event.EventId}/${target}`]);
  }

  public toggleDropdown($event: MouseEvent): void {
    $event.preventDefault();
    $event.stopPropagation();
    this.dropdownStatus.isOpen = !this.dropdownStatus.isOpen;
  }

  public toggleUnusedRooms(): void {
    if (this.hideClosedRooms === true) {
      this.hideClosedRooms = false;
    } else {
      this.hideClosedRooms = true;
    }
  }

  public getCheckedInTotal() {
    if (this.rooms === undefined) {
      return;
    }

    let checkedInTotal = this.rooms.reduce(function (checkedIn, room) {
      checkedIn += room.CheckedIn;
      return checkedIn;
    }, 0);

    return checkedInTotal;
  }

  public getSignedInTotal() {
    if (this.rooms === undefined) {
      return;
    }

    let signedInTotal = this.rooms.reduce(function (signedIn, room) {
      signedIn += room.SignedIn;
      return signedIn;
    }, 0);

    return signedInTotal;
  }

  public getCapacityTotal() {
    if (this.rooms === undefined) {
      return;
    }

    let capacityTotal = this.rooms.reduce(function (capacity, room) {
      capacity += room.Capacity;
      return capacity;
    }, 0);

    return capacityTotal;
  }

  public getVolunteersTotal() {
    if (this.rooms === undefined) {
      return;
    }

    let volunteersTotal = this.rooms.reduce(function (volunteers, room) {
      volunteers += room.Volunteers;
      return volunteers;
    }, 0);

    return volunteersTotal;
  }


  public ngAfterViewChecked() {
    let fixed_table_header = jQuery('.manage-rooms-fixed-header > thead > tr');
    let real_table_header = jQuery('.manage-rooms-scroll-header > thead > tr');

    let real_table_children = real_table_header.children();
    let fixed_table_children = fixed_table_header.children();

    real_table_children.width(function(i, val) {
        fixed_table_children.eq(i).width(real_table_children.eq(i).width());
    });
  }

}
