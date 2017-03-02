import { Component, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { ChildSigninService } from '../child-signin.service';
import { ApiService, RootService } from '../../shared/services';
import { DateOfBirth, EventParticipants, Guest, Group } from '../../shared/models';

import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  selector: 'available-children',
  templateUrl: 'available-children.component.html',
  styleUrls: [ '../scss/_cards.scss', '../scss/_buttons.scss', 'available-children.component.scss' ]
})

export class AvailableChildrenComponent implements OnInit {
  private _eventParticipants: EventParticipants = new EventParticipants();
  private _isServingOneHour = false;
  private _isServingTwoHours = false;
  private isReady = false;
  // tslint:disable:no-unused-variable
  private maxDate: Date = moment().toDate();
  private _newGuestChild: Guest;
  private gradeGroups: Array<Group> = [];
  numberOfMonthsSelection: Array<number>;
  numberOfDaysSelection: Array<number>;
  yearsSelection: Array<number>;
  guestDOB: DateOfBirth = new DateOfBirth();

 @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
 @ViewChild('addGuestModal') public addGuestModal: ModalDirective;

 constructor( private childSigninService: ChildSigninService,
              private route: ActivatedRoute,
              private router: Router,
              private apiService: ApiService,
              private rootService: RootService) { }

 ngOnInit() {
   this.route.params.forEach((params: Params) => {
      let phoneNumber = params['phoneNumber'];
      this.getChildren(phoneNumber);
      this.populateGradeGroups();
    });

    this.populateDatepicker();
 }

 populateDatepicker() {
   this.numberOfMonthsSelection = Array.apply(null, {length: 12}).map(function (e, i) { return i + 1; }, Number);
   this.numberOfDaysSelection = Array.apply(null, {length: 31}).map(function (e, i) { return i + 1; }, Number);
   this.yearsSelection = [];
   let i = 0;
   while (i <= 18) {
     this.yearsSelection.push(moment().subtract(i, 'y').year());
     i++;
   }
 }

 getChildren(phoneNumber) {
   this.childSigninService.getChildrenByPhoneNumber(phoneNumber).subscribe(
       (result) => {
         this.isReady = true;
         this._eventParticipants = result;
       }, (err) => {
         this.isReady = true;
         this.rootService.announceEvent('generalError');
       }
     );
 }

 populateGradeGroups() {
   this.apiService.getGradeGroups().subscribe((groups) => {
       this.gradeGroups = groups;
     },
     error => console.error(error)
   );
 }

 signIn() {
   if (!this._eventParticipants.hasSelectedParticipants()) {
     return this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
   }
   this.isReady = false;
   // remove unselected event participants
   this._eventParticipants.removeUnselectedParticipants();
   this.childSigninService.signInChildren(this._eventParticipants, this.numberEventsAttending).subscribe(
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

 public childrenAvailable(): any[] {
  if (this._eventParticipants) { return this._eventParticipants.Participants; };
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

 get newGuestChild() {
   return this._newGuestChild;
 }

 set newGuestChild(guestChild) {
   this._newGuestChild = guestChild;
 }

 get eventParticipants() {
   return this._eventParticipants;
 }

 set eventParticipants(eventParticipants) {
   this._eventParticipants = eventParticipants;
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
   this._newGuestChild.YearGrade = groupId;
 }

 openNewGuestModal(modal) {
   this.guestDOB = new DateOfBirth();
   this._newGuestChild = new Guest();
   this._newGuestChild.GuestSignin = true;
   this._newGuestChild.Selected = true;
   modal.show();
 }

 datePickerBlur() {
   if (this.guestDOB.year && this.guestDOB.month && this.guestDOB.day) {
     this.newGuestChild.DateOfBirth = moment(`${this.guestDOB.year}-${this.guestDOB.month}-${this.guestDOB.day}`, 'YYYY-M-DD').toDate();
   }

   let needGradeLevelValue = moment(this.newGuestChild.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y'));

    if (needGradeLevelValue === true) {
      this.newGuestChild.YearGrade = -1;
    } else {
      this.newGuestChild.YearGrade = 0;
    }
 }

  saveNewGuest(modal) {
   try {
     this.newGuestChild.FirstName.trim();
     this.newGuestChild.LastName.trim();
   } finally {
     if (!this.newGuestChild.FirstName || !this.newGuestChild.LastName) {
       return this.rootService.announceEvent('echeckChildSigninAddGuestFormInvalid');
     } else if (!this.newGuestChild.DateOfBirth || !moment(this.newGuestChild.DateOfBirth).isValid()) {
       return this.rootService.announceEvent('echeckChildSigninBadDateOfBirth');
     } else if (this.newGuestChild.YearGrade === -1) {
       return this.rootService.announceEvent('echeckNeedValidGradeSelection');
     } else {

       if (this.newGuestChild.YearGrade === 0) {
         this.newGuestChild.YearGrade = undefined;
       }

       this._eventParticipants.Participants.push(this.newGuestChild);
       return modal.hide();
     }
   }
 }

 needGradeLevel(): boolean {
   return moment(this.newGuestChild.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y'));
 }

}
