import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { AdminService } from '../admin.service';
import { Event } from './event';
import { Timeframe } from '../models/timeframe';
import { HeaderService } from '../header/header.service';

import * as moment from 'moment';

@Component({
  selector: 'events',
  templateUrl: 'event-list.component.html'
})
export class EventListComponent implements OnInit {
  events: Event[];
  site: number;
  currentWeekFilter: any;
  weekFilters: Timeframe[];

  constructor(private adminService: AdminService,
              private headerService: HeaderService) {
    // default to Oakley
    this.site = 1;
    this.weekFilters = [];

    // current week
    this.weekFilters.push(this.getWeekObject())
    // next week
    this.weekFilters.push(this.getWeekObject(1))
    // two weeks from now
    this.weekFilters.push(this.getWeekObject(2))
    // default to current week
    this.currentWeekFilter = this.weekFilters[0];

  }

  private getData() {
    this.adminService.getEvents(this.currentWeekFilter.start, this.currentWeekFilter.end, this.site).subscribe(
      events => {
        this.events = events;
      },
      error => console.error(error)
    );
  }

  private getWeekObject(offset = 0): any {
    // add one day so it starts on monday rather than sunday
    return {
        start: moment().add(offset, 'weeks').startOf('week').add(1, 'day'),
        end: moment().add(offset, 'weeks').endOf('week').add(1, 'day')
    }
  }

  ngOnInit() {
    this.getData();
  }

}
