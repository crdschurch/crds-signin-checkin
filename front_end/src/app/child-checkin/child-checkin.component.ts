import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'child-checkin',
  styleUrls: ['child-checkin.component.scss'],
  templateUrl: 'child-checkin.component.html',
})
export class ChildCheckinComponent {

  constructor(private router: Router) {}

  activeStep1() {
    console.log("hi", JSON.stringify(process.env), process.env, process.env.CRDS_API_ENDPOINT);
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
