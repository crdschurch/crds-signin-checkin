import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';
import { NoresultsComponent } from './+noresults';

/*
@Routes ([
  { path: '/noresults', component: NoresultsComponent },
])
*/

@Component({
  moduleId: module.id,
  selector: 'app-search',
  templateUrl: 'search.component.html',
  styleUrls: ['search.component.css'],
  directives: [ROUTER_DIRECTIVES]
})
export class SearchComponent implements OnInit {

  constructor() {}

  ngOnInit() {
  }

}
