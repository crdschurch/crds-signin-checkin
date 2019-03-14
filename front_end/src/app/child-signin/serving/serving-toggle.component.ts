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
  selector: 'serving-toggle',
  templateUrl: 'serving-toggle.component.html',
  styleUrls: [ 'serving-toggle.component.scss' ]
})

export class ServingToggleComponent {
  @Output() setServingHours = new EventEmitter<any>();
  private _isServingOneHour = false;
  private _isServingTwoHours = false;

 @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;

 constructor() { }

 get numberEventsAttending(): number {
   if (this.isServingOneHour) {
     return 1;
   } else if (this.isServingTwoHours) {
     return 2;
   } else {
     return 0;
   }
 }

 get isServing(): boolean {
   return this._isServingOneHour || this._isServingTwoHours;
 }

 get isServingOneHour(): boolean {
   return this._isServingOneHour;
 }

 get isServingTwoHours(): boolean {
   return this._isServingTwoHours;
 }

 set servingOneHour(b) {
   this._isServingTwoHours = false;
   this._isServingOneHour = !this._isServingOneHour;
   this.setServingHours.emit(1);
 }

 set notServing(b) {
   this._isServingOneHour = false;
   this._isServingTwoHours = false;
   this.setServingHours.emit(0);
 }

 set servingTwoHours(b) {
   this._isServingOneHour = false;
   this._isServingTwoHours = !this._isServingTwoHours;
   this.setServingHours.emit(2);
 }

 toggleServicesClick(modal) {
   // if on, turn off
   if (this.isServing) {
     this.notServing = true;
     return true;
   // else if off, open modal to turn on
   } else {
     if (modal) {
       modal.show();
     }
     return false;
   }
 }

 toggleServingHours(modal, hours) {
   if (hours === 1) {
     this.servingOneHour = true;
   } else if (hours === 2) {
     this.servingTwoHours = true;
   }
   if (modal) {
     return modal.hide();
   }
 }

 public showChildModal(): void {
   this.serviceSelectModal.show();
 }

}
