import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService, HttpClientService, RootService, SetupService } from '../../shared/services';
import { Congregation, MachineConfiguration, Event, Timeframe } from '../../shared/models';
import { HeaderService } from '../header/header.service';
import * as moment from 'moment';

@Component({
  selector: 'events',
  templateUrl: 'event-list.component.html'
})
export class EventListComponent implements OnInit {
  private _selectedSiteId: number;
  private _currentWeekFilter: any;
  ready: boolean;
  events: Event[];
  allSites: Congregation[];
  configurationSiteId: number;
  weekFilters: Timeframe[];

  constructor(private apiService: ApiService,
              private headerService: HeaderService,
              private httpClientService: HttpClientService,
              private router: Router,
              private rootService: RootService,
              private setupService: SetupService) {
  }

  private getData() {
    this.apiService.getSites().subscribe(
      (allSites: Congregation[]) => {
        this.allSites = allSites.sort(function(a, b){
            if (a.CongregationName < b.CongregationName) {
              return -1;
            } else if (a.CongregationName > b.CongregationName) {
              return 1;
            }
            return 0;
        })
        // set the initial site to the site from the machine config
        this.selectedSiteId = this.configurationSiteId;
      },
      error => { console.error(error); this.rootService.announceEvent('generalError'); }
    );
  }

  private getWeekObject(offset = 0): any {
    // if today is sunday subtract a day to get the proper start day this is because isoweek does not work.
    // create duplicate copies because the moment object itself changes when you modify it
    let startDay = moment();
    let endDay = moment();
    if (startDay.day() === 0) {
      startDay = startDay.subtract(1, 'day');
      endDay = endDay.subtract(1, 'day');
    }

    // add one day so it starts on monday rather than sunday
    return {
        start: startDay.add(offset, 'weeks').startOf('week').add(1, 'day').toDate(),
        end: endDay.add(offset, 'weeks').endOf('week').add(1, 'day').toDate()
    };
  }

  private createWeekFilters() {
    this.weekFilters = [];
    this.weekFilters.push(new Timeframe(this.getWeekObject(-3)));
    this.weekFilters.push(new Timeframe(this.getWeekObject(-2)));
    this.weekFilters.push(new Timeframe(this.getWeekObject(-1)));
    this.weekFilters.push(new Timeframe(this.getWeekObject()));
    this.weekFilters.push(new Timeframe(this.getWeekObject(1)));
    this.weekFilters.push(new Timeframe(this.getWeekObject(2)));
    this.weekFilters.push(new Timeframe(this.getWeekObject(3)));
    // default to current week
    this.currentWeekFilter = this.weekFilters[3];
  }

  private setupSite(config: MachineConfiguration = null) {
    // default to Oakley (1) if setup cookie is not present or does not have a site id
    this.configurationSiteId = config && config.CongregationId ? config.CongregationId : 1;
  }

  public isReady(): boolean {
    return this.ready;
  }

  get selectedSiteId() {
    return this._selectedSiteId;
  }

  set selectedSiteId(siteId) {
    this.ready = false;
    this._selectedSiteId = siteId;
    this.apiService.getEvents(this._currentWeekFilter.start, this._currentWeekFilter.end, this._selectedSiteId).subscribe(
      events => {
        this.events = Event.fromJsons(events);
        this.ready = true;
      },
      error => { console.error(error); this.rootService.announceEvent('generalError'); }
    );
  }

  get currentWeekFilter() {
    return this._currentWeekFilter;
  }

  get currentWeekFilterId() {
    return this._currentWeekFilter.id;
  }

  set currentWeekFilterId(newWeekFilterId) {
    this.currentWeekFilter = this.weekFilters.filter(wf => {
      return +wf.id === +newWeekFilterId;
    })[0];
  }

  set currentWeekFilter(weekFilter) {
    this.ready = false;
    // this._currentWeekFilterId = weekFilter.id;
    this._currentWeekFilter = weekFilter;
    this.apiService.getEvents(this._currentWeekFilter.start, this._currentWeekFilter.end, this._selectedSiteId).subscribe(
      events => {
        this.events = Event.fromJsons(events);
        this.ready = true;
      },
      error => { console.error(error); this.rootService.announceEvent('generalError'); }
    );
  }

  ngOnInit() {
    this.createWeekFilters();
    this.setupService.getThisMachineConfiguration().subscribe((setupCookie) => {
      this.setupSite(setupCookie);
      this.getData();
    },
    (error) => {
      this.setupSite();
      this.getData();
    });
  }
}
