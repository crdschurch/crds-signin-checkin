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

  required(e) {
    this.optionalParentRequired = false;

    if (this.parents[1].FirstName || this.parents[1].LastName ||
        this.parents[1].PhoneNumber || this.parents[1].EmailAddress) {
      this.optionalParentRequired = true;
    }
  }

  onSubmit(form: NgForm, editMode = false) {
    this.submitted = true;
    if (!form.pristine && form.valid) {
      this.processing = true;

      _.forEach(this.parents, (parent: NewParent): void => {
        parent.CongregationId = this.setupService.getMachineDetailsConfigCookie().CongregationId;
      });

      this.adminService.createNewFamily(this.parents).subscribe((res) => {
        this.rootService.announceEvent('echeckNewFamilyCreated');
        form.resetForm();

        let contacts = (<Contact[]>res.json()).map(r => Contact.fromJson(r));
        let householdId = contacts[0].HouseholdId;

        if (editMode) {
          this.router.navigate(['/admin/events', this.eventId, 'family-finder', householdId, 'edit']);
        } else {
          this.router.navigate(['/admin/events', this.eventId, 'family-finder', householdId]);
        }
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

  private newParent(firstName = '', lastName = '', phone = '', email = ''): NewParent {
    return new NewParent(firstName, lastName, phone, email);
  }

}
