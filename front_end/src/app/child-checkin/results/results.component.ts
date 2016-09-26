import { Component, OnInit, ViewChild } from '@angular/core';
import { ROUTER_DIRECTIVES} from '@angular/router';
import {MODAL_DIRECTIVES, ModalDirective, BS_VIEW_PROVIDERS} from 'ng2-bootstrap/ng2-bootstrap';

/*
@Routes ([
  { path: '/serving-length', component: ServingLengthComponent },
])
*/

@Component({
  selector: 'app-results',
  templateUrl: 'results.component.html',
  directives: [ MODAL_DIRECTIVES, ROUTER_DIRECTIVES ],
  viewProviders: [BS_VIEW_PROVIDERS]
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

  private serving1: boolean = true;

  @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;

  public showServiceSelectModal():void {
    this.serviceSelectModal.show();
  }

  constructor() {}

  ngOnInit() {

  }

}
