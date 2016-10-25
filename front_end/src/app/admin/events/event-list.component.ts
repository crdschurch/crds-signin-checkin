import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { AdminService } from '../admin.service';
import { HttpClientService } from '../../shared/services';
import { Router } from '@angular/router';
import { Subscription }   from 'rxjs/Subscription';

import { Event } from './event';
import { Timeframe } from '../models/timeframe';
import { HeaderService } from '../header/header.service';

import * as moment from 'moment';

@Component({
  selector: 'events',
  templateUrl: 'event-list.component.html',
  providers: [ AdminService, HttpClientService ]
})
export class EventListComponent implements OnInit, OnDestroy {
  events: Event[];
  site: number;
  currentWeekFilter: any;
  weekFilters: Timeframe[];
  subscription: Subscription;
  mission = '<no mission announced>';
  confirmed = false;
  announced = false;
  @Input() astronaut: string;

  constructor(private adminService: AdminService,
              private httpClientService: HttpClientService,
              private router: Router,
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

  announce() {
    this.headerService.announceMission("bob");
  }

  ngOnInit(): void {
    this.getData();
  }

  logout(): void {
    this.httpClientService.logOut();
    this.router.navigate(['/admin/sign-in']);
  }

  ngOnDestroy() {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
  }

}
