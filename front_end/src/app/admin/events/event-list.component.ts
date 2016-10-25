import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { HttpClientService } from '../../shared/services';
import { Router } from '@angular/router';

import { Event } from './event';
import { Timeframe } from '../models/timeframe';

import * as moment from 'moment';

@Component({
  selector: 'events',
  templateUrl: 'event-list.component.html',
  providers: [ AdminService, HttpClientService ]
})
export class EventListComponent implements OnInit {
  events: Event[];
  site: number;
  currentWeekFilter: any;
  weekFilters: Timeframe[];

  constructor(private adminService: AdminService, private httpClientService: HttpClientService, private router: Router) {
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

  private getData(): void {
    this.adminService.getEvents(this.currentWeekFilter.start, this.currentWeekFilter.end, this.site).subscribe(
      events => { this.events = events; },
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

  ngOnInit(): void {
    this.getData();
  }

  logout(): void {
    this.httpClientService.logOut();
    this.router.navigate(['/admin/sign-in']);
  }

}
