import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
// import {Observable} from 'rxjs/Rx';

import { Event, Room } from '../../../shared/models';
import { AdminService } from '../../admin.service';
import { ApiService } from '../../../shared/services';
import { HeaderService } from '../../header/header.service';
import { RootService } from '../../../shared/services/root.service';

import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  templateUrl: 'room-list.component.html',
  styleUrls: ['room-list.component.scss'],
  providers: [ ]
})
export class RoomListComponent implements OnInit {
  rooms: Room[];
  event: Event = null;
  eventId: string;
  public dropdownStatus: { isOpen: boolean, isDisabled: boolean } = { isOpen: false, isDisabled: false };

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
  }

  ngOnInit(): void {
    this.getData();

    // This is temp. until we add websockets to do an actual update
    // We will update the rooms information every 5 seconds
    // Observable.interval(5000)
    //   .mergeMap(() => this.adminService.getRooms(this.eventId))
    //   .subscribe((rooms: Room[]) => {
    //       for (let i = 0; i < rooms.length; i++) {
    //          this.rooms[i].SignedIn = rooms[i].SignedIn;
    //          this.rooms[i].CheckedIn = rooms[i].CheckedIn;
    //        }
    //      },
    //      (error: any) => console.error(error)
    //    );
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
}
