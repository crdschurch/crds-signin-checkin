import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { ChildSigninService } from './child-signin.service';

@Component({
  selector: 'child-signin',
  templateUrl: 'child-signin.component.html',
  styleUrls: ['child-signin.component.scss', 'scss/_stepper.scss' ],
  providers: [ ChildSigninService ]
})
export class ChildSigninComponent {

  clock = Observable.interval(10000).map(() => new Date());

  constructor(private router: Router) {}

  isStepActive(step) {
    if (step === 1) {
      return true;
    } else if (step === 2) {
      if (this.router.url !== '/child-signin/search') {
        return true;
      }
    } else if (step === 3) {
      if (this.router.url === '/child-signin/assignment') {
        return true;
      }
    }
  }

  activeStep1() {
    return this.router.url === '/child-signin/search';
  }

  inRoom() {
    return this.router.url === '/child-signin/room';
  }
}
