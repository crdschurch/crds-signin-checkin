import { Component, OnInit } from '@angular/core';
import { CookieService } from 'angular2-cookie/core';
import { Router } from '@angular/router';
import { SetupService } from '../setup/setup.service';
import { MachineConfiguration } from '../setup/machine-configuration';

@Component({
  selector: 'home',
  templateUrl: 'home.component.html',
})
export class HomeComponent implements OnInit {
  private ready: boolean;

  constructor(private router: Router,
              private cookieService: CookieService,
              private setupService: SetupService) {
    this.ready = false;
  }

  goToChildCheckin() {
    this.router.navigate(['/child-checkin/room']);
  }

  goToChildSignin() {
    this.router.navigate(['/child-signin/search']);
  }

  goToAdminTools() {
    this.router.navigate(['/admin/sign-in']);
  }

  goToSetupError() {
    this.router.navigate(['/setup', {error: true} ]);
  }

  redirectIfConfigured() {
    if (this.setupService.getMachineIdConfigCookie()) {
      this.setupService.getThisMachineConfiguration().subscribe(
          (machineConfig: MachineConfiguration) => {
            this.ready = true;
            if (machineConfig.isTypeSignIn()) {
              return this.goToChildSignin();
            } else if (machineConfig.isTypeRoomCheckin()) {
              return this.goToChildCheckin();
            } else {
              return this.goToSetupError();
            }
          },
          error => {
            this.ready = true;
            return this.goToSetupError();
          }
        );
    } else {
      this.ready = true;
    }
  }

  public isReady() {
    return this.ready;
  }

  ngOnInit() {
    return this.redirectIfConfigured();
  }
}
