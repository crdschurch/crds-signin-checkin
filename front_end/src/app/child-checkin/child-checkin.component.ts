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

  constructor(private setupService: SetupService, private adminService: AdminService) {
    this.kioskDetails = new MachineConfiguration();
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
      },
      error => { console.error(error); }
    );
  }

  isActive(event): boolean {
    return event.EventId === this.selectedEvent.EventId;
  }

  selectEvent(event) {
    // TODO: get new data from backend for event
    console.log('pick event', event);
  }

  public getKioskDetails() {
    return this.kioskDetails;
  }

  public ngOnInit() {
    this.getData();
    this.kioskDetails = this.setupService.getMachineDetailsConfigCookie();
    this.thisSiteName = this.getKioskDetails().CongregationName;
  }

  public showServiceSelectModal() {
    this.serviceSelectModal.show();
  }

  public showChildDetailModal() {
    this.childDetailModal.show();
  }

}
