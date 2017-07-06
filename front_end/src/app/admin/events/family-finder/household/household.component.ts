import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService, RootService } from '../../../../shared/services';
import { AdminService } from '../../../admin.service';
import { DateOfBirth, Child, EventParticipants, Contact, Group, Household } from '../../../../shared/models';

import * as moment from 'moment';

@Component({
  selector: 'household',
  templateUrl: 'household.component.html',
  styleUrls: ['household.component.scss', '../../../../child-signin/scss/_cards.scss']
})
export class HouseholdComponent implements OnInit {
  private householdId: number;
  private _editMode: boolean;
  private processing: boolean;
  private processingAddFamilyMember: boolean;
  private _contact: Contact;
  private gradeGroups: Array<Group> = [];
  private eventId: number;
  private eventTypeId: number;
  household: Household;
  numberOfMonthsSelection: Array<number>;
  numberOfDaysSelection: Array<number>;
  yearsSelection: Array<number>;
  eventParticipants: EventParticipants;
  guestDOB: DateOfBirth = new DateOfBirth();
  numberEventsAttending: number;

  constructor( private apiService: ApiService,
               private adminService: AdminService,
               private rootService: RootService,
               private route: ActivatedRoute,
               private router: Router,
               private location: Location) {}

 ngOnInit() {
   this.eventId = +this.route.snapshot.params['eventId'];
   this.householdId = +this.route.snapshot.params['householdId'];

   this.apiService.getEvent(String(this.eventId)).subscribe((event) => {
     this.eventTypeId = event.EventTypeId;
     this.getChildren();
   }, error => console.error(error));

   this.adminService.getHouseholdInformation(this.householdId).subscribe((household) => {
     this.household = household;
   }, error => console.error(error));

   this.populateGradeGroups();
   this.populateDatepicker();
 }

 private getChildren() {
   this.processing = true;
   this.adminService.getChildrenByHousehold(+this.householdId, +this.eventId).subscribe((ep: EventParticipants) => {
     this.eventParticipants = ep;
     this.eventParticipants.Participants.forEach(p => p.Selected = true && p.canCheckIn(this.eventTypeId));
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
   this.apiService.getGradeGroups(this.eventId).subscribe((groups) => {
       this.gradeGroups = groups;
     },
     error => console.error(error)
   );
 }

 get editMode() {
   if (this.contact) {
     return this.contact.ContactId;
   }
 }

 addGuestChild(guestChild) {
   this.eventParticipants.Participants.push(guestChild);
 }

 setServingHours(hours) {
   this.numberEventsAttending = hours;
 }

 signIn() {
    if (!this.eventParticipants.hasSelectedParticipants()) {
      return this.rootService.announceEvent('echeckSigninNoParticipantsSelected');
    }
    this.processing = true;
    // remove unselected event participants
    this.eventParticipants.removeUnselectedParticipants();
    this.adminService.findFamilySigninAndPrint(this.eventParticipants, this.numberEventsAttending).subscribe(
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

 openNewFamilyMemberModal(modal, existingContact: Contact) {
   if (existingContact) {
     this.contact = Contact.fromJson(existingContact);
     this.guestDOB = new DateOfBirth(moment(existingContact.DateOfBirth).month() + 1,
      moment(existingContact.DateOfBirth).date(), moment(existingContact.DateOfBirth).year());
   } else {
     this.guestDOB = new DateOfBirth();
     this.contact = new Contact();
     this.contact.HouseholdId = +this.householdId;
   }
   modal.show();
 }

 back() {
   this.location.back();
 }

 get maleGenderId(): number {
   return Contact.genderIdMale();
 }

 get femaleGenderId(): number {
   return Contact.genderIdFemale();
 }

 get contact() {
   return this._contact;
 }

 set contact(contact) {
   this._contact = contact;
 }

 datePickerBlur() {
   if (this.guestDOB.year && this.guestDOB.month && this.guestDOB.day) {
     this.contact.DateOfBirth = moment(`${this.guestDOB.year}-${this.guestDOB.month}-${this.guestDOB.day}`, 'YYYY-M-DD').toDate();
   }
   let needGradeLevelValue = moment(this.contact.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y'));

    if (needGradeLevelValue) {
      this.contact.YearGrade = -1;
    } else {
      this.contact.YearGrade = 0;
    }
 }

 needGradeLevel(): boolean {
   return moment(this.contact.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y').add(1, 'd'));
 }

 updateContactYearGradeGroup(contact: Contact, groupId: number) {
   contact.YearGrade = groupId;
 }

 saveNewFamilyMember(modal) {
  try {
    this.processingAddFamilyMember = true;
    this.contact.Nickname.trim();
    this.contact.LastName.trim();
    this.contact.FirstName = this.contact.Nickname;
    this.contact.DisplayName = `${this.contact.LastName}, ${this.contact.Nickname}`;
  } finally {
    if (!this.contact.Nickname || !this.contact.LastName) {
      this.processingAddFamilyMember = false;
      return this.rootService.announceEvent('echeckChildSigninAddGuestFormInvalid');
    } else if (!this.contact.DateOfBirth || !moment(this.contact.DateOfBirth).isValid()) {
      this.processingAddFamilyMember = false;
      return this.rootService.announceEvent('echeckChildSigninBadDateOfBirth');
    } else if (this.contact.YearGrade === -1) {
      this.processingAddFamilyMember = false;
      return this.rootService.announceEvent('echeckNeedValidGradeSelection');
    } else if (this.contact.GenderId !== Contact.genderIdMale() && this.contact.GenderId !== Contact.genderIdFemale()) {
      this.processingAddFamilyMember = false;
      return this.rootService.announceEvent('echeckNeedValidGenderSelection');
    } else if (!this.contact.ContactId && this.contact.IsSpecialNeeds === undefined) {
      // only check this for new contacts
      this.processingAddFamilyMember = false;
      return this.rootService.announceEvent('echeckNeedSpecialNeedsSelection');
    } else {
      if (+this.contact.YearGrade < 1) {
        this.contact.YearGrade = undefined;
      }
      if (this.contact.ContactId) {
        this.adminService.updateFamilyMember(this.contact).subscribe(
          (response: EventParticipants) => {
            this.announceSuccess(modal);
            this.rootService.announceEvent('echeckEditFamilyMemberSuccess');
          }, (err) => {
            this.announceError();
          }
        );
      } else {
        this.adminService.addFamilyMembers([ this.contact ], this.householdId).subscribe(
          (response: EventParticipants) => {
            this.announceSuccess(modal);
            this.rootService.announceEvent('echeckAddFamilyMemberSuccess');
          }, (err) => {
            this.announceError();
          }
        );
      }
    }
  }
 }

 announceSuccess(modal) {
   this.processingAddFamilyMember = false;
   this.getChildren();
   return modal.hide();
 }

 announceError() {
   this.processingAddFamilyMember = false;
   this.rootService.announceEvent('generalError');
 }

}
