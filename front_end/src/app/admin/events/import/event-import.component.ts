import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Event } from '../event';
import { HeaderService } from '../../header/header.service';
import { AdminService } from '../../admin.service';

import * as moment from 'moment';

@Component({
  selector: '.event-import',
  templateUrl: 'event-import.component.html',
  styleUrls: ['event-import.component.scss'],
  providers: [ ],
})
export class EventImportComponent implements OnInit {
  targetEvent: Event;
  events: Event[];
  sourceEventDate: Date;

  constructor(private route: ActivatedRoute,
    private adminService: AdminService,
    private headerService: HeaderService) {
  }

  ngOnInit(): void {
    let eventId = this.route.snapshot.params['eventId'];

    this.adminService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.headerService.announceEvent(event);
      this.sourceEventDate = moment(event.EventStartDate).startOf('day').subtract(7, 'days').toDate();
      this.changeSourceEventDate(null);
    });
  }

  public changeSourceEventDate($event: any): void {
    this.adminService
      .getEvents(this.sourceEventDate, this.sourceEventDate, this.targetEvent.EventSiteId)
      .subscribe((events: Event[]) => {
        // Sort the source events by date & time
        this.events = events.sort((a: Event, b: Event) => {
          return a.EventStartDate.localeCompare(b.EventStartDate);
        });
      });
  }
}
