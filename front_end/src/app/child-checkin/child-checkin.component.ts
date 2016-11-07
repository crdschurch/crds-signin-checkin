import { Component, Injectable, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { SetupService } from '../setup/setup.service';
import { AdminService } from '../admin/admin.service';
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

  clock = Observable.interval(1000).map(() => new Date());
  thisSiteName: string;
  todaysEvents: Event[];

  constructor(private setupService: SetupService, private adminService: AdminService) {
    this.kioskDetails = new MachineConfiguration();
  }

  private getData() {
    let today = new Date();
    this.adminService.getEvents(today, today).subscribe(
      events => {
        this.todaysEvents = events;
      },
      error => { console.error(error); }
    );
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
