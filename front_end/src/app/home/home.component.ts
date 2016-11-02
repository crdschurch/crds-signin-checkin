import { Component, OnInit } from '@angular/core';
import { CookieService } from 'angular2-cookie/core';
import { Router } from '@angular/router';
import { MachineConfiguration } from '../setup/machine-configuration';

@Component({
  selector: 'home',
  templateUrl: 'home.component.html',
})
export class HomeComponent implements OnInit {

  constructor(private router: Router, private cookieService: CookieService) { }

  goToChildSignin() {
    this.router.navigate(['/child-signin/search']);
  }

  goToAdminTools() {
    // console.log("child sign in")
    this.router.navigate(['/admin/sign-in']);
  }

  redirectIfConfigured() {
    const machineConfig = MachineConfiguration.fromJson( this.cookieService.getObject(MachineConfiguration.COOKIE_NAME) );
    if (machineConfig.isTypeSignIn()) {
      return this.goToChildSignin();
    }
  }

  ngOnInit() {
    return this.redirectIfConfigured();
  }
}
