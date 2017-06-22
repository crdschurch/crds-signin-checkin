import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';

import { ApiService, RootService } from '../../../../../shared/services';
import { HeaderService } from '../../../../header/header.service';
import { AdminService } from '../../../../admin.service';
import { Group, Household, State, Country, Contact, DateOfBirth, EventParticipants } from '../../../../../shared/models';

import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  selector: 'family-member-entry',
  templateUrl: 'family-member-entry.component.html'
})
export class FamilyMemberEntryComponent implements OnInit {
  private maskPhoneNumber: any = [/[1-9]/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/];
  private loading: boolean;
  private processing: boolean;
  private processingAddFamilyMember: boolean;
  private _contacts: Array<Contact>;
  private gradeGroups: Array<Group> = [];
  private eventId: number;
  private numberOfMonthsSelection: Array<number>;
  private numberOfDaysSelection: Array<number>;
  private yearsSelection: Array<number>;
  private householdId: number;
  guestDOB: DateOfBirth = new DateOfBirth();

  constructor( private apiService: ApiService,
               private adminService: AdminService,
               private route: ActivatedRoute,
               private headerService: HeaderService,
               private rootService: RootService,
               private router: Router) {}

  ngOnInit() {
    this.loading = true;
    this.processing = false;
    this.eventId = +this.route.snapshot.params['eventId'];
    this.householdId = +this.route.snapshot.params['householdId'];

    this.guestDOB = new DateOfBirth();
    this.contacts = [ new Contact() ];
    this.contacts[0].HouseholdId = +this.householdId;

    this.apiService.getEvent(String(this.eventId)).subscribe((event) => {
      this.headerService.announceEvent(event);
    }, error => console.error(error));

    this.populateGradeGroups();
    this.populateDatepicker();
  }

  get maleGenderId(): number {
    return Contact.genderIdMale();
  }

  get femaleGenderId(): number {
    return Contact.genderIdFemale();
  }

  get contacts() {
    return this._contacts;
  }

  set contacts(contacts: Array<Contact>) {
    this._contacts = contacts;
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

  datePickerBlur(contact) {
    if (this.guestDOB.year && this.guestDOB.month && this.guestDOB.day) {
      contact.DateOfBirth = moment(`${this.guestDOB.year}-${this.guestDOB.month}-${this.guestDOB.day}`, 'YYYY-M-DD').toDate();
    }
    let needGradeLevelValue = moment(contact.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y'));

    if (needGradeLevelValue) {
      contact.YearGrade = -1;
    } else {
      contact.YearGrade = 0;
    }
  }

  needGradeLevel(contact): boolean {
    return moment(contact.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y').add(1, 'd'));
  }

  updateContactYearGradeGroup(contact: Contact, groupId: number) {
    contact.YearGrade = groupId;
  }

  saveNewFamilyMember() {
    let isValid = true;

    _.forEach(this.contacts, (contact) => {
      try {
        this.processingAddFamilyMember = true;
        contact.Nickname.trim();
        contact.LastName.trim();
        contact.FirstName = contact.Nickname;
        contact.DisplayName = `${contact.LastName}, ${contact.Nickname}`;
      } finally {
        if (!contact.Nickname || !contact.LastName) {
          this.processingAddFamilyMember = false;
          this.rootService.announceEvent('echeckChildSigninAddGuestFormInvalid');

          isValid = false;
          return false;
        } else if (!contact.DateOfBirth || !moment(contact.DateOfBirth).isValid()) {
          this.processingAddFamilyMember = false;
          this.rootService.announceEvent('echeckChildSigninBadDateOfBirth');

          isValid = false;
          return false;
        } else if (contact.YearGrade === -1) {
          this.processingAddFamilyMember = false;
          this.rootService.announceEvent('echeckNeedValidGradeSelection');

          isValid = false;
          return false;
        } else if (contact.GenderId !== Contact.genderIdMale() && contact.GenderId !== Contact.genderIdFemale()) {
          this.processingAddFamilyMember = false;
          this.rootService.announceEvent('echeckNeedValidGenderSelection');

          isValid = false;
          return false;
        } else if (!contact.ContactId && contact.IsSpecialNeeds === undefined) {
          // only check this for new contacts
          this.processingAddFamilyMember = false;
          this.rootService.announceEvent('echeckNeedSpecialNeedsSelection');

          isValid = false;
          return false;
        } else {
          if (+contact.YearGrade < 1) {
            contact.YearGrade = undefined;
          }
        }
      }
    });

    if (isValid) {
      this.adminService.addFamilyMembers(this.contacts, this.householdId).subscribe(
        (response: EventParticipants) => {
          this.announceSuccess();
          this.rootService.announceEvent('echeckAddFamilyMemberSuccess');
        }, (err) => {
          this.announceError();
        }
      );
    }
  }

  announceSuccess() {
    this.processingAddFamilyMember = false;
    this.router.navigate(['..']);
  }

  announceError() {
    this.processingAddFamilyMember = false;
    this.rootService.announceEvent('generalError');
  }
}
