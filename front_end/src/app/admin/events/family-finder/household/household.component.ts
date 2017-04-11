import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService, RootService } from '../../../../shared/services';
import { AdminService } from '../../../admin.service';
import { DateOfBirth, Child, EventParticipants, Contact, Group } from '../../../../shared/models';
import { HeaderService } from '../../../header/header.service';

import * as moment from 'moment';

@Component({
  selector: 'household',
  templateUrl: 'household.component.html',
  styleUrls: ['household.component.scss', '../../../../child-signin/scss/_cards.scss']
})
export class HouseholdComponent implements OnInit {
  private eventId: number;
  private householdId: number;
  private processing: boolean;
  private processingAddFamilyMember: boolean;
  private _newContact: Contact;
  private gradeGroups: Array<Group> = [];
  numberOfMonthsSelection: Array<number>;
  numberOfDaysSelection: Array<number>;
  yearsSelection: Array<number>;
  eventParticipants: EventParticipants;
  guestDOB: DateOfBirth = new DateOfBirth();

  constructor( private apiService: ApiService,
               private adminService: AdminService,
               private rootService: RootService,
               private route: ActivatedRoute,
               private router: Router,
               private headerService: HeaderService) {}

 ngOnInit() {
   this.eventId = +this.route.snapshot.params['eventId'];
   this.householdId = +this.route.snapshot.params['householdId'];

   this.apiService.getEvent(String(this.eventId)).subscribe((event) => {
     this.headerService.announceEvent(event);
   }, error => console.error(error));

   this.getChildren();
   this.populateGradeGroups();
   this.populateDatepicker();
 }

 private getChildren() {
   this.processing = true;
   this.adminService.getChildrenByHousehold(+this.householdId).subscribe((ep: EventParticipants) => {
     this.eventParticipants = ep;
     if (this.eventParticipants === undefined || !this.eventParticipants.hasParticipants()) {
       this.rootService.announceEvent('echeckFamilyFinderNoChildren');
     }
     this.processing = false;
   }, (err) => {
     if (err === 'No current events for site') {
       this.rootService.announceEvent('noCurrentEvent');
     } else {
       this.rootService.announceEvent('generalError');
     }
     this.processing = false;
   });
 }

 private populateDatepicker() {
   this.numberOfMonthsSelection = Array.apply(null, {length: 12}).map(function (e, i) { return i + 1; }, Number);
   this.numberOfDaysSelection = Array.apply(null, {length: 31}).map(function (e, i) { return i + 1; }, Number);
   this.yearsSelection = [];
   let i = 0;
   while (i <= 18) {
     this.yearsSelection.push(moment().subtract(i, 'y').year());
     i++;
   }
 }

 private populateGradeGroups() {
   this.apiService.getGradeGroups().subscribe((groups) => {
       this.gradeGroups = groups;
     },
     error => console.error(error)
   );
 }

 signIn() {
    if (!this.eventParticipants.hasSelectedParticipants()) {
      return this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
    }
    this.processing = true;
    // remove unselected event participants
    this.eventParticipants.removeUnselectedParticipants();
    const numberEventsAttending = 1;
    this.adminService.findFamilySigninAndPrint(this.eventParticipants, numberEventsAttending).subscribe(
      (response: EventParticipants) => {
        this.processing = false;
        if (response && response.Participants && response.Participants.length > 0) {
          this.router.navigate([`/admin/events/${this.eventId}/family-finder`]);
          this.rootService.announceEvent('echeckFamilyFinderSignedIn');
        } else {
          this.rootService.announceEvent('generalError');
        }
      }, (err) => {
        this.rootService.announceEvent('generalError');
        this.processing = false;
      }
    );
 }

 openNewFamilyMemberModal(modal) {
   this.guestDOB = new DateOfBirth();
   this._newContact = new Contact();
   this._newContact.IsSpecialNeeds = false;
   this._newContact.GenderId = Contact.genderIdMale();
   this._newContact.HouseholdId = +this.householdId;
   modal.show();
 }

 get maleGenderId(): number {
   return Contact.genderIdMale();
 }

 get femaleGenderId(): number {
   return Contact.genderIdFemale();
 }

 get newContact() {
   return this._newContact;
 }

 set newContact(newContact) {
   this._newContact = newContact;
 }

 datePickerBlur() {
   if (this.guestDOB.year && this.guestDOB.month && this.guestDOB.day) {
     this.newContact.DateOfBirth = moment(`${this.guestDOB.year}-${this.guestDOB.month}-${this.guestDOB.day}`, 'YYYY-M-DD').toDate();
   }
   let needGradeLevelValue = moment(this.newContact.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y'));

    if (needGradeLevelValue) {
      this.newContact.YearGrade = -1;
    } else {
      this.newContact.YearGrade = 0;
    }
 }

 needGradeLevel(): boolean {
   return moment(this.newContact.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y').add(1, 'd'));
 }

 updateContactYearGradeGroup(contact: Contact, groupId: number) {
   contact.YearGrade = groupId;
 }


 saveNewFamilyMember(modal) {
  //  console.log('save modal', this.newContact)
  try {
    this.processingAddFamilyMember = true;
    this.newContact.FirstName.trim();
    this.newContact.LastName.trim();
  } finally {
    if (!this.newContact.FirstName || !this.newContact.LastName) {
      return this.rootService.announceEvent('echeckChildSigninAddGuestFormInvalid');
    } else if (!this.newContact.DateOfBirth || !moment(this.newContact.DateOfBirth).isValid()) {
      return this.rootService.announceEvent('echeckChildSigninBadDateOfBirth');
    } else if (this.newContact.YearGrade === -1) {
      return this.rootService.announceEvent('echeckNeedValidGradeSelection');
    } else {

      if (+this.newContact.YearGrade < 1) {
        this.newContact.YearGrade = undefined;
      }
      this.adminService.addFamilyMember(this.newContact).subscribe(
        (response: EventParticipants) => {
          this.processingAddFamilyMember = false;
          this.getChildren();
          return modal.hide();
        }, (err) => {
          this.processingAddFamilyMember = false;
          this.rootService.announceEvent('generalError');
        }
      );
    }
  }
 }


}
