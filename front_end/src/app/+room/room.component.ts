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

  constructor() {}

  ngOnInit() {
  }

  public shownumberSearchModal():void {
    this.numberSearchModal.show();
  }

  public hidenumberSearchModal():void {
    this.numberSearchModal.hide();
  }

}
