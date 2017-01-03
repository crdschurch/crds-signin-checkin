import { Component, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { ChildSigninService } from '../child-signin.service';
import { RootService } from '../../shared/services';
import { EventParticipants, Child, Guest } from '../../shared/models';

import * as moment from 'moment';

@Component({
  selector: 'available-children',
  templateUrl: 'available-children.component.html',
  styleUrls: [ '../scss/_cards.scss', '../scss/_buttons.scss', 'available-children.component.scss' ]
})

export class AvailableChildrenComponent implements OnInit {
  private eventParticipants: EventParticipants;
  private _isServingOneHour: boolean = false;
  private _isServingTwoHours: boolean = false;
  private isReady: boolean = false;
  private maxDate: Date = moment().toDate();
  private newGuestChild: Guest;

 @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
 @ViewChild('addGuestModal') public addGuestModal: ModalDirective;

 constructor( private childSigninService: ChildSigninService,
              private route: ActivatedRoute,
              private router: Router,
              private rootService: RootService) { }

 ngOnInit() {
   this.route.params.forEach((params: Params) => {
      let phoneNumber = params['phoneNumber'];
      this.childSigninService.getChildrenByPhoneNumber(phoneNumber).subscribe(
        (result) => {
          this.isReady = true;
          this.eventParticipants = result;
        }, (err) => {
          this.isReady = true;
          this.rootService.announceEvent('generalError');
        }
      );
    });
 }

 signIn() {
   if (!this.eventParticipants.hasSelectedParticipants()) {
     this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
     return;
   }

   this.isReady = false;

   this.childSigninService.signInChildren(this.eventParticipants, this.numberEventsAttending).subscribe(
     (response: EventParticipants) => {
       this.isReady = true;
       if (response && response.Participants && response.Participants.length > 0) {
         this.router.navigate(['/child-signin/assignment']);
       } else {
         this.rootService.announceEvent('generalError');
       }
     }, (err) => {
       this.isReady = true;
       this.rootService.announceEvent('generalError');
     }
   );
 }

 public childrenAvailable(): Child[] {
   if (this.eventParticipants) { return this.eventParticipants.Participants };
 }

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
 }

 set notServing(b) {
   this._isServingOneHour = false;
   this._isServingTwoHours = false;
 }

 set servingTwoHours(b) {
   this._isServingOneHour = false;
   this._isServingTwoHours = !this._isServingTwoHours;
 }

 toggleServingHours(modal, hours) {
   if (hours === 1) {
     this.servingOneHour = true;
   } else if (hours === 2) {
     this.servingTwoHours = true;
   }
   if (modal) {
     modal.hide();
   }
 }

 public showChildModal(): void {
   this.serviceSelectModal.show();
 }
 
 toggleClick(modal) {
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

 updateChildYearGradeGroup(guest: Guest, groupId: number) {
   guest.YearGrade = groupId;
 }

 openNewGuestModal(modal) {
   this.newGuestChild = new Guest();
   this.newGuestChild.DateOfBirth = moment().startOf('day').toDate();
   modal.show();
 }

 needGradeLevel(guest: Guest): boolean {
   return moment(guest.DateOfBirth).isBefore(moment().startOf('day').subtract(4, 'y'));
 }

}
