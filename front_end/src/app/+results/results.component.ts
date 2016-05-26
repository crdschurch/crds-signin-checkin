import { Component, OnInit } from '@angular/core';
import { ServingLengthComponent } from './+serving-length';
import { Routes , ROUTER_DIRECTIVES} from '@angular/router';

@Routes ([
  { path: '/serving-length', component: ServingLengthComponent },
])

@Component({
  moduleId: module.id,
  selector: 'app-results',
  templateUrl: 'results.component.html',
  styleUrls: ['results.component.css'],
  directives: [ROUTER_DIRECTIVES]
})
@Routes([
  {path: '/serving-length', component: ServingLengthComponent}
])
export class ResultsComponent implements OnInit {

  constructor() {}

  ngOnInit() {
  }

}
