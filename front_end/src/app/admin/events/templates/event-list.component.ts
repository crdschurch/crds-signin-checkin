import { Component, OnInit } from '@angular/core';
import { ApiService, RootService, SetupService } from '../../../shared/services';
import { Congregation, MachineConfiguration, Event, Timeframe } from '../../../shared/models';
import * as moment from 'moment';

@Component({
  selector: 'events',
  templateUrl: '../event-list.component.html'
})
export class EventTemplatesListComponent implements OnInit {
  private ready = false;
  events: Event[];
  configurationSiteId: number;
  isEventTemplates = true;

  constructor(private apiService: ApiService,
              private rootService: RootService,
              private setupService: SetupService) {
  }

  ngOnInit() {
      this.ready = false;
      this.setupService.getThisMachineConfiguration().subscribe((setupCookie) => {
        const configurationSiteId = setupCookie && setupCookie.CongregationId ? setupCookie.CongregationId : 1;
        this.apiService.getEventTemplates(configurationSiteId).subscribe(
          events => {
            this.events = Event.fromJsons(events);
            this.ready = true;
          },
          error => { console.error(error); this.rootService.announceEvent('generalError'); }
        );
      });
  }

  public isReady(): boolean {
    return this.ready;
  }
}
