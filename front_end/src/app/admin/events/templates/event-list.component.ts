import { Component, OnInit } from '@angular/core';
import { ApiService, RootService, SetupService } from '../../../shared/services';
import { Congregation, MachineConfiguration, Event, Timeframe } from '../../../shared/models';
import * as moment from 'moment';

@Component({
  selector: 'events',
  templateUrl: '../event-list.component.html'
})
export class EventTemplatesListComponent implements OnInit {
  // private _selectedSiteId: number;
  // private _currentWeekFilter: any;
  ready: boolean;
  events: Event[];
  // allSites: Congregation[];
  configurationSiteId: number;
  isEventTemplates = true;

  constructor(private apiService: ApiService,
              private rootService: RootService,
              private setupService: SetupService) {
  }

  ngOnInit() {
      // this.configurationSiteId = config && config.CongregationId ? config.CongregationId : 1;
      // this.apiService.getEventTemplates(this._selectedSiteId).subscribe(
      //   events => {
      //     this.events = Event.fromJsons(events);
      //     this.ready = true;
      //   },
      //   error => { console.error(error); this.rootService.announceEvent('generalError'); }
      // );
      console.log("templates event list init")
  }
  public isReady(): boolean {
    return true;
  }
}
