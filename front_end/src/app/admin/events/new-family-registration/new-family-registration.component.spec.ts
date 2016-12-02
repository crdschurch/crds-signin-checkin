import { Observable } from 'rxjs';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

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
  it('#updateNumberOfKids', () => {
    fixture.setUp();
    fixture.updateNumberOfKids(4);
    expect(fixture.family.children.length).toEqual(4);
  });
  it('#onSubmit success', () => {
    spyOn(fixture, 'setUp');
    fixture.onSubmit();
    expect(adminService.createNewFamily).toHaveBeenCalled();
  });
  it('#onSubmit error', () => {
    (<jasmine.Spy>(adminService.createNewFamily)).and.returnValue(Observable.throw('Error'));
    fixture.onSubmit();
    expect(rootService.announceEvent).toHaveBeenCalledWith('generalError');
  });

});