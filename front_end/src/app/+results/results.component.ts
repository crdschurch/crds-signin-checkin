import { Component, OnInit } from '@angular/core';
import { ServingLengthComponent } from './+serving-length';
import { ROUTER_DIRECTIVES} from '@angular/router';

/*
@Routes ([
  { path: '/serving-length', component: ServingLengthComponent },
])
*/

@Component({
  moduleId: module.id,
  selector: 'app-results',
  templateUrl: 'results.component.html',
  styleUrls: ['results.component.css'],
  directives: [ROUTER_DIRECTIVES]
})

export class ResultsComponent implements OnInit {

  private cb1: boolean = true;
  private cb2: boolean = true;
  private cb3: boolean = false;
  private cb4: boolean = true;
  private cb5: boolean = true;
  private cb6: boolean = true;
  private cb7: boolean = true;
  private cb8: boolean = true;
  private cb9: boolean = true;
  private cb10: boolean = true;

  constructor() {}

  ngOnInit() {
  }

}
