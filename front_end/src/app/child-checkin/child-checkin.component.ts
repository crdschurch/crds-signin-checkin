import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'child-checkin',
  templateUrl: 'child-checkin.component.html',
  styleUrls: ['child-checkin.component.scss', 'scss/_stepper.scss' ]
})
export class ChildCheckinComponent {

  constructor(private router: Router) {}

  activeStep1() {
    console.log("environment variables:", "ECHECK_API_TOKEN:", process.env.ECHECK_API_TOKEN, " ECHECK_API_ENDPOINT:", process.env.ECHECK_API_ENDPOINT, " CRDS_CMS_ENDPOINT:", process.env.CRDS_CMS_ENDPOINT);
    return this.router.url === '/child-checkin';
  }

  activeStep2() {
    return this.router.url === '/child-checkin/results';
  }

  activeStep3() {
    return this.router.url === '/child-checkin/assignment';
  }

  inRoom() {
    return this.router.url === '/child-checkin/room';
  }
}
