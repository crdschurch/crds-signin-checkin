import { Component, OnInit } from '@angular/core';
import { CookieService } from 'angular2-cookie/core';
import { Router } from '@angular/router';
import { SetupService } from '../setup/setup.service';

@Component({
  selector: 'home',
  templateUrl: 'home.component.html',
})
export class HomeComponent implements OnInit {

  constructor(private router: Router,
              private cookieService: CookieService,
              private setupService: SetupService) {
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
          machineConfig => {
            return this.goToChildSignin();
          },
          error => {
            return this.goToSetupError();
          }
        );
    }
  }

  ngOnInit() {
    return this.redirectIfConfigured();
  }
}
