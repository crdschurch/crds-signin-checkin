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

  activeStep1() {
    return this.router.url === '/child-signin';
  }

  activeStep2() {
    return this.router.url === '/child-signin/results';
  }

  activeStep3() {
    return this.router.url === '/child-signin/assignment';
  }

  inRoom() {
    return this.router.url === '/child-signin/room';
  }
}
