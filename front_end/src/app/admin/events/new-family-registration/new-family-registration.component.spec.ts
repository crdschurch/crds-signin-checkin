import { Observable } from 'rxjs';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { Child, NewChild, NewParent, NewFamily } from '../../../shared/models';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import * as moment from 'moment';

const event = {
  'EventTitle': 'Cool Event 83',
  'EventId': '92398420398',
};
let router;
let adminService = jasmine.createSpyObj('adminService', [, 'createNewFamily', 'getUser']);
let rootService = jasmine.createSpyObj('rootService', ['announceEvent']);
let setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
let route: ActivatedRoute;
route = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };
let fixture;

describe('NewFamilyRegistrationComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(adminService.createNewFamily)).and.returnValue(Observable.of());
    (<jasmine.Spy>(rootService.announceEvent)).and.returnValue(Observable.of());
    (<jasmine.Spy>(setupService.getMachineDetailsConfigCookie)).and.returnValue({});

    fixture = new NewFamilyRegistrationComponent(route, adminService, rootService, setupService, router);
    fixture.family = new NewFamily();
    fixture.family.children = [];
    fixture.family.children[0] = new Child();
    fixture.family.children[0].DateOfBirth = new Date();
    fixture.family.children[1] = new Child();
    fixture.family.children[1].DateOfBirth = new Date();
  });
  it('#ngOnInit', () => {
    spyOn(fixture, 'setUp');
    fixture.ngOnInit();
    expect(fixture.setUp).toHaveBeenCalled();
  });
  it('#setUp', () => {
    fixture.setUp();
    expect(fixture.parents).toBeDefined(2);
  });
  it('#onSubmit success', () => {
    let form = {
      pristine: false,
      valid: true
    };
    (<jasmine.Spy>(adminService.createNewFamily)).and.returnValue(Observable.throw('Error'));
    spyOn(fixture, 'setUp');
    fixture.onSubmit(form);
    expect(adminService.createNewFamily).toHaveBeenCalled();
  });
  it('#onSubmit error', () => {
    let form = {
      pristine: false,
      valid: true
    };
    fixture.onSubmit(form);
    expect(rootService.announceEvent).toHaveBeenCalledWith('generalError');
  });
  describe('#checkIfEmailExists', () => {
    beforeEach(() => {
    });
    it('should show error and disable button if email exists', () => {
      (<jasmine.Spy>(adminService.getUser)).and.returnValue(Observable.of({HouseholdId: 123}));
      fixture = new NewFamilyRegistrationComponent(route, adminService, rootService, setupService, router);
      let p = new NewParent();
      p.EmailAddress = 'exists@email.com';
      fixture.checkIfEmailExists(p, 0);
      expect(p.DuplicateEmail).toBeTruthy();
      expect(p.HouseholdId).toBeTruthy();
      expect(fixture.duplicateEmailProcessing.length).toEqual(0);

    });
    it('should not show error if email doesnt exist', () => {
      (<jasmine.Spy>(adminService.getUser)).and.returnValue(Observable.of(null));
      fixture = new NewFamilyRegistrationComponent(route, adminService, rootService, setupService, router);
      let p = new NewParent();
      p.EmailAddress = 'new@email.com';
      fixture.checkIfEmailExists(p, 0);
      expect(p.DuplicateEmail).toBeFalsy();
      expect(p.HouseholdId).toBeFalsy();
      expect(fixture.duplicateEmailProcessing.length).toEqual(0);
    });
  });
  it('should return true when child > 5 years old', () => {
    fixture = new NewFamilyRegistrationComponent(route, adminService, rootService, setupService, router);
    let child = new NewChild();
    child.DateOfBirth = moment().subtract(5, 'years').subtract(1, 'day').toDate();
    expect(fixture.needGradeLevel(child)).toBeTruthy();
  });

  it('should return false when child < 3 years old', () => {
    fixture = new NewFamilyRegistrationComponent(route, adminService, rootService, setupService, router);
    let child = new NewChild();
    child.DateOfBirth = moment().subtract(3, 'years').add(1, 'day').toDate();
    expect(fixture.needGradeLevel(child)).toBeFalsy();
  });
  describe('validation', () => {
    let child;
    let parent;
    beforeEach(() => {
      fixture = new NewFamilyRegistrationComponent(route, adminService, rootService, setupService, router);
      child = new NewChild();
      parent = new NewParent();
    });
    describe('date of birth', () => {
      it('should delete after input if bad date', () => {
        child.DateOfBirthString = '02-30-201_';
        fixture.onDateBlur({target : {value: child.DateOfBirthString} },  child);
        expect(child.DateOfBirth).toBeFalsy();
        expect(fixture.needGradeLevel(child)).toBeFalsy();
      });
      it('should delete after input if bad date', () => {
        child.DateOfBirthString = '02-30-2011';
        fixture.onDateBlur({target : {value: child.DateOfBirthString} },  child);
        expect(child.DateOfBirth).toBeFalsy();
        expect(fixture.needGradeLevel(child)).toBeFalsy();
      });
      it('should create date of birth when valid date', () => {
        child.DateOfBirthString = '02-09-2011';
        fixture.onDateBlur({target : {value: child.DateOfBirthString} },  child);
        expect(child.DateOfBirth).toBeDefined();
        expect(fixture.needGradeLevel(child)).toBeTruthy();
      });
    });
    describe('phoneNumber', () => {
      it('should delete after input if bad phone number', () => {
        parent.PhoneNumber = '513-554-542_';
        fixture.onPhoneBlur({target : {value: parent.PhoneNumber} },  parent);
        expect(parent.PhoneNumber).toBeFalsy();
      });
      it('should accept if valid phone number', () => {
        parent.PhoneNumber = '513-554-5423';
        fixture.onPhoneBlur({target : {value: parent.PhoneNumber} },  parent);
        expect(parent.PhoneNumber).toBeTruthy();
      });
    });
  });
});
