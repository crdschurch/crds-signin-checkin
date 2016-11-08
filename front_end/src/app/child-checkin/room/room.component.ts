import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

import { Child } from '../../shared/models/child';
import { ChildCheckinService } from '../child-checkin.service';
import { RootService } from '../../shared/services';

@Component({
  selector: 'room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss', ]
})

export class RoomComponent implements OnInit {
  @ViewChild('numberSearchModal') public numberSearchModal: ModalDirective;
  @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
  @ViewChild('childDetailModal') public childDetailModal: ModalDirective;

  private _children: Array<Child> = [];

  constructor(private childCheckinService: ChildCheckinService, private rootService: RootService) {}

  ngOnInit() {
    this.childCheckinService.getChildrenForRoom(1820, 4525342).subscribe((children) => {
      this.children = children;
    }, (error) => {
      console.error(error);
      this.rootService.announceEvent('generalError');
    });
  }

  checkedIn(): Array<Child> {
    return this.children.filter( (obj: Child) => { return obj.checkedIn(); } );
  }

  signedIn(): Array<Child> {
    return this.children.filter( (obj: Child) => { return !obj.checkedIn(); } );
  }

  toggleCheckIn(child: Child) {
    this.childCheckinService.checkInChildren(child).subscribe(() => {
    }, (error) => {
      console.error(error);
      this.rootService.announceEvent('generalError');
    });
  }

  get children(): Array<Child> {
    return this._children;
  }

  set children(child: Array<Child>) {
    this._children = child;
  }

  public showNumberSearchModal(): void {
    this.numberSearchModal.show();
  }

  public showServiceSelectModal(): void {
    this.serviceSelectModal.show();
  }

  public showChildDetailModal(): void {
    this.childDetailModal.show();
  }
}
