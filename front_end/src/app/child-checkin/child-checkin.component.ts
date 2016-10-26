import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ChildCheckinService } from './child-checkin.service';

@Component({
  selector: 'child-checkin',
  templateUrl: 'child-checkin.component.html',
  styleUrls: ['child-checkin.component.scss', 'scss/_stepper.scss' ],
  providers: [ ChildCheckinService ]
})
export class ChildCheckinComponent {

  constructor(private router: Router) {}

  activeStep1() {
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
