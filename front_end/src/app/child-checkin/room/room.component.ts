import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

@Component({
  selector: 'room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss', ]
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
