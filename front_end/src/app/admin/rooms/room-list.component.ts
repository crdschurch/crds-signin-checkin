import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from '../admin.service';
import { Event } from '../events/event';
import { Room } from './room';
import { HeaderService } from '../header/header.service';
import { RootService } from '../../shared/services/root.service';

import * as moment from 'moment';

@Component({
  templateUrl: 'room-list.component.html',
  styleUrls: ['room-list.component.scss'],
  providers: [ ]
})
export class RoomListComponent implements OnInit {
  rooms: Room[];
  event: Event = null;
  eventId: string;
  public disabled: boolean = false;
  public status: {isopen: boolean} = {isopen: false};

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
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
    this.adminService.getEvent(this.eventId).subscribe(
      event => {
        this.event = event;
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );
  }
  ngOnInit(): void {
    this.getData();
  }

  public okToImportOrReset($event: MouseEvent, target: string): void {
    if (this.event && moment(this.event.EventStartDate).isBefore(moment())) {
      this.rootService.announceEvent('echeckCantImportToPastEvent');
      return;
      // $event.preventDefault();
      // $event.stopPropagation();
    }

    this.router.navigate([`/admin/events/${this.eventId}/${target}`]);
  }

  // Tate - dropdown demo
  public toggled(open: boolean): void {
    console.log('Dropdown is now: ', open);
  }

  public toggleDropdown($event: MouseEvent): void {
    $event.preventDefault();
    $event.stopPropagation();
    this.status.isopen = !this.status.isopen;
  }
}
