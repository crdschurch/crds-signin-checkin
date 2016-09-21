import { Component, ViewContainerRef } from '@angular/core';
import { provideRouter, RouterConfig, Router, ROUTER_DIRECTIVES } from '@angular/router';
import { SearchComponent } from './search';
import { ResultsComponent } from './+results';
import { AssignmentComponent } from './+assignment';
import { GuestComponent } from './+guest';
import { RoomComponent } from './+room';

export const routes: RouterConfig =  [
  { path: '', pathMatch: 'full', redirectTo: '/search' },
  { path: 'search', component: SearchComponent },
  { path: 'results', component: ResultsComponent },
  { path: 'room', component: RoomComponent },
  { path: 'assignment', component: AssignmentComponent },
  { path: 'guest', component: GuestComponent }
];

export const APP_ROUTER_PROVIDERS = [
 provideRouter(routes)
];

@Component({
  moduleId: module.id,
  selector: 'front-end-app',
  templateUrl: 'front-end.component.html',
  styleUrls: ['front-end.component.css'],
  directives: [ROUTER_DIRECTIVES]
})
export class FrontEndAppComponent {
  title = 'The Angular 2 app works!';

  constructor(
    private router: Router,
    public viewContainerRef:ViewContainerRef) {
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
