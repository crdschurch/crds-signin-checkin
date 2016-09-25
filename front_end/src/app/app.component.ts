import { Component, ViewContainerRef } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app',
  templateUrl: 'app.component.html',
})
export class AppComponent {
  title = 'The Angular 2 app works!';

  constructor(private router: Router) {
  }

  ngOnInit() {
    this.router.navigate([window.location.pathname]);
  }
  activeStep1() {
    return window.location.pathname=="/";
  }
  activeStep2() {
    return window.location.pathname=="/results";
  }
  activeStep3() {
    return window.location.pathname=="/assignment";
  }
  inRoom() {
    return window.location.pathname=="/room";
  }
}

