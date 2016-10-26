import { Component, ViewEncapsulation, ViewContainerRef } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent {

  private viewContainerRef: ViewContainerRef;

  constructor(private router: Router, viewContainerRef: ViewContainerRef) {
    // You need this small hack in order to catch application root view container ref
    this.viewContainerRef = viewContainerRef;
  }

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
