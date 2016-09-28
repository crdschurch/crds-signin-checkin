import { Component, OnInit, ViewChild } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';
import { NoresultsComponent } from './+noresults';
import {MODAL_DIRECTIVES, ModalDirective, BS_VIEW_PROVIDERS} from 'ng2-bootstrap/ng2-bootstrap';

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
  directives: [ROUTER_DIRECTIVES, MODAL_DIRECTIVES],
  viewProviders: [BS_VIEW_PROVIDERS]
})
export class SearchComponent implements OnInit {
  @ViewChild('unmatchedNumberModal') public unmatchedNumberModal: ModalDirective;

  constructor() {}

  ngOnInit() {
  }

  public showUnmatchedNumberModal():void {
    this.unmatchedNumberModal.show();
  }

}
