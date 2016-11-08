import { Component, Injectable, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { SetupService } from '../setup/setup.service';
import { AdminService } from '../admin/admin.service';
import { Event } from '../admin/events/event';
import { MachineConfiguration } from '../setup/machine-configuration';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'child-checkin',
  templateUrl: 'child-checkin.component.html',
  styleUrls: ['child-checkin.component.scss', 'scss/_stepper.scss' ],
  providers: []
})
@Injectable()
export class ChildCheckinComponent implements OnInit {
  @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
  @ViewChild('childDetailModal') public childDetailModal: ModalDirective;
  private kioskDetails: MachineConfiguration;

  clock = Observable.interval(10000).map(() => new Date());
  thisSiteName: string;
  selectedEvent: Event;
  todaysEvents: Event[];
  ready: boolean;

  constructor(private setupService: SetupService, private adminService: AdminService) {
    this.kioskDetails = new MachineConfiguration();
    this.ready = false;
  }

  private getData() {
    let today = new Date();
    this.adminService.getEvents(today, today).subscribe(
      events => {
        this.todaysEvents = [];
        // transform to Event
        for (let event of events) {
          this.todaysEvents.push(Event.fromJson(event));
        }
        if (this.todaysEvents && this.todaysEvents.length) {
          for (let event of this.todaysEvents) {
            if (event.IsCurrentEvent) {
              this.selectedEvent = event;
              break;
            }
          }
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

  selectEvent(event) {
    this.selectedEvent = event;
    // TODO: populate UI with new data from backend for event
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

  public showChildDetailModal() {
    this.childDetailModal.show();
  }

}
