import { Component, Injectable, OnInit } from '@angular/core';
import { SetupService } from '../setup/setup.service';
import { MachineConfiguration } from '../setup/machine-configuration';

@Component({
  selector: 'child-checkin',
  templateUrl: 'child-checkin.component.html',
  styleUrls: ['child-checkin.component.scss', 'scss/_stepper.scss' ],
  providers: []
})
@Injectable()
export class ChildCheckinComponent implements OnInit {
  private kioskDetails: MachineConfiguration;

  constructor(private setupService: SetupService) {
    this.kioskDetails = new MachineConfiguration();
  }

  public getKioskDetails() {
    return this.kioskDetails;
  }

  public ngOnInit() {
    this.kioskDetails = this.setupService.getMachineDetailsConfigCookie();
  }
}
