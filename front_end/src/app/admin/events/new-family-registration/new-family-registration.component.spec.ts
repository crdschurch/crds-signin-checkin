import { Observable } from 'rxjs';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { NewChild } from '../../../shared/models';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import * as moment from 'moment';

const event = {
  'EventTitle': 'Cool Event 83',
  'EventId': '92398420398',
};
let router;
let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let adminService = jasmine.createSpyObj('adminService', ['getGradeGroups', 'createNewFamily']);
let headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
let rootService = jasmine.createSpyObj('rootService', ['announceEvent']);
let route: ActivatedRoute;
route = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };
let fixture;

describe('NewFamilyRegistrationComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
    (<jasmine.Spy>(adminService.getGradeGroups)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.createNewFamily)).and.returnValue(Observable.of());
    (<jasmine.Spy>(rootService.announceEvent)).and.returnValue(Observable.of());
    fixture = new NewFamilyRegistrationComponent(route, apiService, headerService, adminService, rootService, router);
  });
  it('#ngOnInit', () => {
    spyOn(fixture, 'setUp');
    fixture.ngOnInit();
    expect(fixture.setUp).toHaveBeenCalled();
  });
  it('#setUp', () => {
    fixture.setUp();
    expect(fixture.family).toBeDefined();
    expect(fixture.family.parent).toBeDefined(1);
    expect(fixture.family.children.length).toEqual(1);
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
    expect(apiService.getEvent).toHaveBeenCalledWith(event.EventId);
    expect(adminService.getGradeGroups).toHaveBeenCalled();
    expect(headerService.announceEvent).toHaveBeenCalledWith(event);
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
  it('should return true when child > 5 years old', () => {
    fixture = new NewFamilyRegistrationComponent(route, apiService, headerService, adminService, rootService, router);
    let child = new NewChild();
    child.DateOfBirth = moment().subtract(5, 'years').subtract(1, 'day').toDate();
    expect(fixture.needGradeLevel(child)).toBeTruthy();
  });

  it('should return false when child < 5 years old', () => {
    fixture = new NewFamilyRegistrationComponent(route, apiService, headerService, adminService, rootService, router);
    let child = new NewChild();
    child.DateOfBirth = moment().subtract(5, 'years').add(1, 'day').toDate();
    expect(fixture.needGradeLevel(child)).toBeFalsy();
  });
});
