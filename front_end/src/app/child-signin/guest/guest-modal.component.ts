import { Component, Output, ViewChild, EventEmitter } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { ChildSigninService } from '../child-signin.service';
import { ApiService, RootService } from '../../shared/services';
import { DateOfBirth, EventParticipants, Guest, Group } from '../../shared/models';
import { Constants } from '../../shared/constants';

import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  selector: 'guest-modal',
  templateUrl: 'guest-modal.component.html',
  styleUrls: [ 'guest-modal.component.scss' ]
})

export class GuestModalComponent {
  @Output() addGuestChild = new EventEmitter<any>();
  showGuestOption = false;
  private _newGuestChild: Guest;
//   @Output() setServingHours = new EventEmitter<any>();
//   private _isServingOneHour = false;
//   private _isServingTwoHours = false;

  @ViewChild('addGuestModal') public addGuestModal: ModalDirective;

 constructor() {
   this.showGuestOption = true;
   //this.newGuestChild = true;
  }

//  public showChildModal(): void {
//    debugger;
//    this.addGuestModal.show();
//  }

  showChildModal(modal) {
   // if on, turn off
  //  if (this.isServing) {
  //    this.notServing = true;
  //    return true;
  //  // else if off, open modal to turn on
  //  } else {
  //    if (modal) {
  //      modal.show();
  //    }
  //    return false;
  //  }
  debugger;

  if (modal) {
    modal.show();
    this.addGuestChild.emit(undefined);
  }
 }

  get newGuestChild() {
   return this._newGuestChild;
 }

 set newGuestChild(guestChild) {
   this._newGuestChild = guestChild;
 }


//  get numberEventsAttending(): number {
//    if (this.isServingOneHour) {
//      return 1;
//    } else if (this.isServingTwoHours) {
//      return 2;
//    } else {
//      return 0;
//    }
//  }

//  get isServing(): boolean {
//    return this._isServingOneHour || this._isServingTwoHours;
//  }

//  get isServingOneHour(): boolean {
//    return this._isServingOneHour;
//  }

//  get isServingTwoHours(): boolean {
//    return this._isServingTwoHours;
//  }

//  set servingOneHour(b) {
//    this._isServingTwoHours = false;
//    this._isServingOneHour = !this._isServingOneHour;
//    this.setServingHours.emit(1);
//  }

//  set notServing(b) {
//    this._isServingOneHour = false;
//    this._isServingTwoHours = false;
//    this.setServingHours.emit(0);
//  }

//  set servingTwoHours(b) {
//    this._isServingOneHour = false;
//    this._isServingTwoHours = !this._isServingTwoHours;
//    this.setServingHours.emit(2);
//  }

//  toggleServicesClick(modal) {
//    // if on, turn off
//    if (this.isServing) {
//      this.notServing = true;
//      return true;
//    // else if off, open modal to turn on
//    } else {
//      if (modal) {
//        modal.show();
//      }
//      return false;
//    }
//  }

//  toggleServingHours(modal, hours) {
//    if (hours === 1) {
//      this.servingOneHour = true;
//    } else if (hours === 2) {
//      this.servingTwoHours = true;
//    }
//    if (modal) {
//      return modal.hide();
//    }
//  }

//  public showChildModal(): void {
//    this.serviceSelectModal.show();
//  }

}
