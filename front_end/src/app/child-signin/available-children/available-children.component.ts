import { Component, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { ChildSigninService } from '../child-signin.service';
import { ApiService, RootService } from '../../shared/services';
import { DateOfBirth, EventParticipants, Guest, Group } from '../../shared/models';
import { Constants } from '../../shared/constants';

import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  selector: 'available-children',
  templateUrl: 'available-children.component.html',
  styleUrls: [ 'available-children.component.scss', '../scss/_cards.scss', '../scss/_buttons.scss', '../scss/_stepper.scss' ]
})

export class AvailableChildrenComponent implements OnInit {
  private _eventParticipants: EventParticipants = new EventParticipants();
  private isReady = false;
  private _numberEventsAttending: number;
  private maxDate: Date = moment().toDate();
  private _newGuestChild: Guest;
  private gradeGroups: Array<Group> = [];
  private eventTypeId: number;
  private eventId: number;
  numberOfMonthsSelection: Array<number>;
  numberOfDaysSelection: Array<number>;
  yearsSelection: Array<number>;
  guestDOB: DateOfBirth = new DateOfBirth();
  showGuestOption = false;
  showServingOption = false;

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

 addGuestChild(guestChild) {
   this.eventParticipants.Participants.push(guestChild);
 }

 setServingHours(hours) {
   this._numberEventsAttending = hours;
 }

 get numberEventsAttending() {
   return this._numberEventsAttending;
 }

 set numberEventsAttending(hours) {
   this._numberEventsAttending = hours;
 }

 getChildren(phoneNumber) {
   this.childSigninService.getChildrenByPhoneNumber(phoneNumber).subscribe(
       (result) => {
         this.isReady = true;
         this._eventParticipants = result;
         this.eventId = this._eventParticipants.CurrentEvent.EventId;
         this.eventTypeId = this._eventParticipants.CurrentEvent.EventTypeId;
         this.setServingAndGuestDisplay();
       }, (err) => {
         this.isReady = true;
         this.rootService.announceEvent('generalError');
       }
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
         if (this.eventParticipants.CurrentEvent.isStudentMinistry) {
          this.router.navigate(['/child-signin/search']);
         } else {
          this.router.navigate(['/child-signin/assignment']);
         }
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

 get eventParticipants() {
   return this._eventParticipants;
 }

 set eventParticipants(eventParticipants) {
   this._eventParticipants = eventParticipants;
 }

  setServingAndGuestDisplay() {
    if (this.eventParticipants.CurrentEvent.isStudentMinistry) {
      this.showGuestOption = true;
      this.showServingOption = false;
    } else if (this.eventParticipants.CurrentEvent.isChildCare) {
      this.showGuestOption = false;
      this.showServingOption = false;
    } else {
      this.showGuestOption = true;
      this.showServingOption = true;
    }
  }

}
