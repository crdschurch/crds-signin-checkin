import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService, HttpClientService, RootService, SetupService } from '../../shared/services';
import { MachineConfiguration, Event, Timeframe } from '../../shared/models';
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

  constructor(private apiService: ApiService,
              private headerService: HeaderService,
              private httpClientService: HttpClientService,
              private router: Router,
              private rootService: RootService,
              private setupService: SetupService) {
  }

  private getData() {
    this.apiService.getEvents(this.currentWeekFilter.start, this.currentWeekFilter.end, this.site).subscribe(
      events => {
        this.events = events;
      },
      error => { console.error(error); this.rootService.announceEvent('generalError'); }
    );
  }

  private getWeekObject(offset = 0): any {
    // add one day so it starts on monday rather than sunday
    return {
        start: moment().add(offset, 'weeks').startOf('week').add(1, 'day'),
        end: moment().add(offset, 'weeks').endOf('week').add(1, 'day')
    };
  }

  private createWeekFilters() {
    this.weekFilters = [];

    // current week
    this.weekFilters.push(this.getWeekObject());
    // next week
    this.weekFilters.push(this.getWeekObject(1));
    // two weeks from now
    this.weekFilters.push(this.getWeekObject(2));
    // default to current week
    this.currentWeekFilter = this.weekFilters[0];
  }

  private setupSite(config: MachineConfiguration) {
    // default to Oakley (1) if setup cookie is not present or does not have a site id
    this.site = config && config.CongregationId ? config.CongregationId : 1;
  }

  public isReady(): boolean {
    return this.events !== undefined;
  }

  ngOnInit(): void {
    this.createWeekFilters();
    this.setupService.getThisMachineConfiguration().subscribe((setupCookie) => {
      this.setupSite(setupCookie);
      this.getData();
    },
    (error) => {
      this.setupSite(null);
      this.getData();
    });
  }
}
