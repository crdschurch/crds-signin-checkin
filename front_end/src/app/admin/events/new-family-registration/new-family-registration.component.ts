// tslint:disable:no-unused-variable

import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { NewParent, NewChild, Contact } from '../../../shared/models';
import { AdminService } from '../../admin.service';
import { ApiService, RootService, SetupService } from '../../../shared/services';
import { HeaderService } from '../../header/header.service';

import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  styleUrls: ['new-family-registration.component.scss'],
  selector: 'new-family-registration',
  templateUrl: 'new-family-registration.component.html'
})
export class NewFamilyRegistrationComponent implements OnInit {
  private maskPhoneNumber: any = [/[1-9]/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/];
  private maskDate: any = [/[0-1]/, /[0-9]/, '/', /[0-3]/, /\d/, '/', /[1,2]/, /[0,9]/, /[0,1,2,8,9]/, /\d/];
  private eventId: string;
  private processing: boolean;
  private submitted: boolean;
  private parents: Array<NewParent> = [];
  private optionalParentRequired = false;
  // this is an array of the indexes of the parents in process of
  // being checked (so async calls work for all parents)
  // for instance if second parent is in process of checking for
  // duplicate email it will be [1]
  duplicateEmailProcessing = [];

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private adminService: AdminService,
    private rootService: RootService,
    private setupService: SetupService,
    private router: Router) {}

  ngOnInit() {
    this.setUp();
  }

  setUp() {
    this.processing = false;
    this.submitted = false;
    this.eventId = this.route.snapshot.params['eventId'];
    this.createParents();

    this.apiService.getEvent(this.eventId).subscribe((event) => {
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );
  }

  createParents() {
    let newParent;
    if (this.route.snapshot.queryParams) {
      newParent = this.newParent(
        this.route.snapshot.queryParams['first'],
        this.route.snapshot.queryParams['last'],
        this.route.snapshot.queryParams['phone'],
        this.route.snapshot.queryParams['email']
      );
    } else {
      newParent = this.newParent();
    }
    this.parents = [newParent];
    this.parents.push(this.newParent());
  }

  get maleGenderId(): number {
    return NewParent.genderIdMale();
  }

  get femaleGenderId(): number {
    return NewParent.genderIdFemale();
  }

  needGradeLevel(child: NewChild): boolean {
    return moment(child.DateOfBirth).isBefore(moment().startOf('day').subtract(3, 'y').add(1, 'd'));
  }

  updateChildYearGradeGroup(child: NewChild, groupId: number) {
    child.YearGrade = groupId;
  }

  onPhoneBlur(e, parent) {
    try {
      if (parent.PhoneNumber.indexOf('_') > -1) {
        e.target.value = '';
        parent.PhoneNumber = undefined;
      }
    } catch (e) { }
  }

  onDateBlur(e, child) {
    if (child.DateOfBirthString.indexOf('_') === -1 && moment(child.DateOfBirthString, 'MM/DD/YYYY').isValid()) {
      child.DateOfBirth = moment(child.DateOfBirthString, 'MM/DD/YYYY').toDate();
    } else {
      delete child.DateOfBirthString;
      e.target.value = '';
    }
  }

  requiredOnBlur(e) {
    this.optionalParentRequired = false;

    if (!(this.parents[1].FirstName === '' || this.parents[1].FirstName === undefined || this.parents[1].FirstName === null) ||
          !(this.parents[1].LastName === '' || this.parents[1].LastName === undefined || this.parents[1].LastName === null) ||
          !(this.parents[1].PhoneNumber === '' || this.parents[1].PhoneNumber === undefined || this.parents[1].PhoneNumber === null) ||
          !(this.parents[1].EmailAddress === '' || this.parents[1].EmailAddress === undefined || this.parents[1].EmailAddress === null)) {
      this.optionalParentRequired = true;
    }
  }

  onSubmit(form: NgForm, editMode = false) {
    this.submitted = true;
    if (!form.pristine && form.valid) {
      this.processing = true;

      let tmpParents = this.parents.filter((parent: NewParent) => {
        return !(parent.FirstName === '' || parent.FirstName === undefined || parent.FirstName === null);
      });

      tmpParents.map((parent: NewParent) => {
        parent.CongregationId = this.setupService.getMachineDetailsConfigCookie().CongregationId;
      });

      this.adminService.createNewFamily(tmpParents).subscribe((res) => {
        this.rootService.announceEvent('echeckNewFamilyCreated');
        form.resetForm();

        let contacts = (<Contact[]>res.json()).map(r => Contact.fromJson(r));
        let householdId = contacts[0].HouseholdId;

        this.router.navigate(['/admin/events', this.eventId, 'family-finder', householdId, 'edit', {newFamily: true}]);
      }, (error) => {
        switch (error.status) {
          case 412:
            this.rootService.announceEvent('echeckNewFamilyAdminSetupWrong');
            break;
          default:
            this.rootService.announceEvent('generalError');
            break;
          }
        this.processing = false;
      });
    }
  }

  checkIfEmailExists(parent: NewParent, parentIndex: number) {
    this.duplicateEmailProcessing.push(parentIndex);
    this.adminService.getUser(parent.EmailAddress).subscribe(
      (res: any) => {
        if (res) {
          parent.DuplicateEmail = parent.EmailAddress;
          parent.HouseholdId = res.Household_ID;
        } else {
          parent.DuplicateEmail = undefined;
          parent.HouseholdId = undefined;
        }
        this.duplicateEmailProcessing.splice(this.duplicateEmailProcessing.indexOf(parentIndex), 1);
      }, (error) => {
        console.error(error);
      });
  }

  isCheckingEmailExists() {
    return this.duplicateEmailProcessing.length > 0;
  }

  areDuplicateEmails() {
    return _.find(this.parents, (o) => { return o.DuplicateEmail; });
  }

  private newParent(firstName = '', lastName = '', phone = '', email = ''): NewParent {
    return new NewParent(firstName, lastName, phone, email);
  }

}
