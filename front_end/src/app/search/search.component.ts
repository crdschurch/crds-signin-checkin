import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';
import { NoresultsComponent } from './noresults';

/*
@Routes ([
  { path: '/noresults', component: NoresultsComponent },
])
*/

@Component({
  selector: 'app-search',
  templateUrl: 'search.component.html',
  directives: [ROUTER_DIRECTIVES]
})
export class SearchComponent implements OnInit {

  constructor() {}

  ngOnInit() {
  }

}
