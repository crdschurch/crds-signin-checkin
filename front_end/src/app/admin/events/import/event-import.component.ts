import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Event } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { ApiService } from '../../../shared/services';

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
    private apiService: ApiService,
    private headerService: HeaderService) {
  }

  ngOnInit(): void {
    let eventId = this.route.snapshot.params['eventId'];

    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.headerService.announceEvent(event);
      this.sourceEventDate = moment(event.EventStartDate).startOf('day').subtract(7, 'days').toDate();
      this.changeSourceEventDate(null);
    });
  }

  public changeSourceEventDate($event: any): void {
    this.apiService
      .getEvents(this.sourceEventDate, this.sourceEventDate, this.targetEvent.EventSiteId)
      .subscribe((events: Event[]) => {
        // Sort the source events by date & time
        this.events = events.sort((a: Event, b: Event) => {
          return a.EventStartDate.localeCompare(b.EventStartDate);
        });
      });
  }
}
