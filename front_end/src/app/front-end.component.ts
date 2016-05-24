import { Component } from '@angular/core';
import { Router, Routes, ROUTER_DIRECTIVES } from '@angular/router';
import { SearchComponent } from './search';
import { ResultsComponent } from './+results';
import { AssignmentComponent } from './+assignment';

@Routes ([
  { path: '/', component: SearchComponent },
  { path: '/results', component: ResultsComponent },
  { path: '/assignment', component: AssignmentComponent }

])

@Component({
  moduleId: module.id,
  selector: 'front-end-app',
  templateUrl: 'front-end.component.html',
  styleUrls: ['front-end.component.css'],
  directives: [ROUTER_DIRECTIVES, SearchComponent, ResultsComponent]
})
export class FrontEndAppComponent {
  title = 'The Angular 2 app works!';

  constructor(private router: Router) {}

  ngOnInit() {
    this.router.navigate([window.location.pathname]);
  }
}
