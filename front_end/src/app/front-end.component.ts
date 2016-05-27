import { Component } from '@angular/core';
import { Router, Routes, ROUTER_DIRECTIVES } from '@angular/router';
import { SearchComponent } from './search';
import { ResultsComponent } from './+results';
import { AssignmentComponent } from './+assignment';
import { GuestComponent } from './+guest';

@Routes ([
  { path: '/', component: SearchComponent },
  { path: '/search', component: SearchComponent },
  { path: '/results', component: ResultsComponent },
  { path: '/assignment', component: AssignmentComponent },
  { path: '/guest', component: GuestComponent }
])

@Component({
  moduleId: module.id,
  selector: 'front-end-app',
  templateUrl: 'front-end.component.html',
  styleUrls: ['front-end.component.css'],
  directives: [ROUTER_DIRECTIVES]
})
export class FrontEndAppComponent {
  title = 'The Angular 2 app works!';

  constructor(private router: Router) {}

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
}
