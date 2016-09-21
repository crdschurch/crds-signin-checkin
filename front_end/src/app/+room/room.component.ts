import { Component, OnInit, ViewChild } from '@angular/core';
import {MODAL_DIRECTIVES, ModalDirective, BS_VIEW_PROVIDERS} from 'ng2-bootstrap/ng2-bootstrap';

@Component({
  moduleId: module.id,
  selector: 'app-room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.css'],
  directives: [  MODAL_DIRECTIVES ],
  viewProviders: [BS_VIEW_PROVIDERS]
})

export class RoomComponent implements OnInit {
  @ViewChild('numberSearchModal') public numberSearchModal: ModalDirective;
  @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
  @ViewChild('childDetailModal') public childDetailModal: ModalDirective;

  constructor() {}

  ngOnInit() {
  }

  public showNumberSearchModal():void {
    this.numberSearchModal.show();
  }

  public showServiceSelectModal():void {
    this.serviceSelectModal.show();
  }

  public showChildDetailModal():void {
    this.childDetailModal.show();
  }
}
