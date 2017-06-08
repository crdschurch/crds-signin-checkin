import { Component, Output, Input, ViewChild, EventEmitter, OnInit } from '@angular/core';
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
  styleUrls: [ 'guest-modal.component.scss', '../scss/_cards.scss' ]
})

export class GuestModalComponent implements OnInit {
  @Output() addGuestChild = new EventEmitter<any>();
  @ViewChild('addGuestModal') public addGuestModal: ModalDirective;
  @Input() eventTypeId: number;
  @Input() eventId: number;
  showGuestOption = false;
  private _newGuestChild: Guest;
  numberOfMonthsSelection: Array<number>;
  numberOfDaysSelection: Array<number>;
  yearsSelection: Array<number>;
  guestDOB: DateOfBirth = new DateOfBirth();
  private gradeGroups: Array<Group> = [];

  constructor(private apiService: ApiService, private rootService: RootService) {

  }

  ngOnInit() {
   this.showGuestOption = true;
   this._newGuestChild = new Guest();
   this.populateDatepicker();
   this.populateGradeGroups();
 }

  showChildModal(modal) {
    if (modal) {
      this.guestDOB = new DateOfBirth();
      this._newGuestChild = new Guest();
      this._newGuestChild.GuestSignin = true;
      this._newGuestChild.Selected = true;
      modal.show();
    }
  }

  get newGuestChild() {
    return this._newGuestChild;
  }

 set newGuestChild(guestChild) {
   this._newGuestChild = guestChild;
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

 saveNewGuest(addGuestModal) {
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

        this.addGuestChild.emit(this._newGuestChild);
        addGuestModal.hide();
      }
    }
  }

  needGradeLevel(): boolean {
    return moment(this.newGuestChild.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y'));
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

  setFirstName(value) {
   this.newGuestChild.FirstName = value;
 }

 setLastName(value) {
   this.newGuestChild.LastName = value;
 }

  populateGradeGroups() {
   this.apiService.getGradeGroups(this.eventId).subscribe((groups) => {
       this.gradeGroups = groups;
     },
     error => console.error(error)
   );
 }

  updateChildYearGradeGroup(guest: Guest, groupId: number) {
   this._newGuestChild.YearGrade = +groupId;
 }

 checkSMEventTypeId() {
   if (this.eventTypeId === Constants.StudentMinistry6through8EventType ||
    this.eventTypeId === Constants.StudentMinistry9through12EventType ||
    this.eventTypeId === Constants.BigEventType) {
     return true;
   } else {
     return false;
   }
 }

}
