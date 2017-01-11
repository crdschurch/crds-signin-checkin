import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { ApiService, SetupService } from '../shared/services';
import { Observable } from 'rxjs/Observable';

import { Event, MachineConfiguration } from '../shared/models';
import { ChildCheckinService } from './child-checkin.service';

@Component({
  selector: 'child-checkin',
  templateUrl: 'child-checkin.component.html',
  styleUrls: ['child-checkin.component.scss', 'scss/_stepper.scss' ],
  providers: [ ChildCheckinService ]
})
export class ChildCheckinComponent implements OnInit {
  @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
  @ViewChild('childDetailModal') public childDetailModal: ModalDirective;
  @ViewChild('childSearchModal') public childSearchModal: ModalDirective;
  private kioskDetails: MachineConfiguration;

  clock = Observable.interval(10000).map(() => new Date());
  thisSiteName: string;
  todaysEvents: Event[];
  ready: boolean;

  constructor(private setupService: SetupService, private apiService: ApiService,  private childCheckinService: ChildCheckinService) {
    this.kioskDetails = new MachineConfiguration();
    this.ready = false;
  }

  private getData() {
    let today = new Date();
    this.apiService.getEvents(today, today).subscribe(
      events => {
        this.todaysEvents = [];
        // transform to Event
        for (let event of events) {
          this.todaysEvents.push(Event.fromJson(event));
        }

        if (this.todaysEvents && this.todaysEvents.length) {
          // Sort by date
          this.todaysEvents = this.todaysEvents.sort((a: Event, b: Event) => {
            return a.EventStartDate.localeCompare(b.EventStartDate);
          });

          // Set current service
          this.selectedEvent = this.todaysEvents.find(e => e.IsCurrentEvent);

          // if no current service, pick the first one in list
          if (!this.selectedEvent) {
            this.selectedEvent = this.todaysEvents[0];
          }
        }
        this.ready = true;
      },
      error => {
        console.error(error);
        this.ready = true;
      }
    );
  }

  isActive(event): boolean {
    return event.EventId === this.selectedEvent.EventId;
  }

  get selectedEvent(): Event {
    return this.childCheckinService.selectedEvent;
  }

  set selectedEvent(event) {
    this.childCheckinService.selectedEvent = event;
  }

  public getKioskDetails() {
    return this.kioskDetails;
  }

  public ngOnInit() {
    this.getData();
    this.kioskDetails = this.setupService.getMachineDetailsConfigCookie();
    this.thisSiteName = this.getKioskDetails() ? this.getKioskDetails().CongregationName : null;
  }

  public showServiceSelectModal() {
    this.serviceSelectModal.show();
  }

  public hideServiceSelectModal() {
    this.serviceSelectModal.hide();
  }

  public showChildDetailModal() {
    this.childDetailModal.show();
  }

  public showChildSearchModal() {
    this.childSearchModal.show();
  }

  public hideChildSearchModal() {
    this.childSearchModal.hide();
  }
}
