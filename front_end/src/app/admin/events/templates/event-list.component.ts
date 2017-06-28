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
  private ready = false;
  events: Event[];
  // allSites: Congregation[];
  configurationSiteId: number;
  isEventTemplates = true;

  constructor(private apiService: ApiService,
              private rootService: RootService,
              private setupService: SetupService) {
  }

  ngOnInit() {
      this.ready = false;
      this.apiService.getEventTemplates(13).subscribe(
        events => {
          this.events = Event.fromJsons(events);
          this.ready = true;
        },
        error => { console.error(error); this.rootService.announceEvent('generalError'); }
      );
  }
  public isReady(): boolean {
    return this.ready;
  }
}
