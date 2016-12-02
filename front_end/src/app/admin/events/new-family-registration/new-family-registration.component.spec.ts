import { Observable } from 'rxjs';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { NewFamily, NewParent, NewChild } from '../../../shared/models';
import * as moment from 'moment';

const event = {
  'EventTitle': 'Cool Event 83',
  'EventId': '92398420398',
};
const eventTitle = 'Cool Event';
let router;
let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let adminService = jasmine.createSpyObj('adminService', ['getGradeGroups']);
let headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
let rootService;
let route: ActivatedRoute;
route = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };
let fixture;

fdescribe('NewFamilyRegistrationComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
    (<jasmine.Spy>(adminService.getGradeGroups)).and.returnValue(Observable.of());
    fixture = new NewFamilyRegistrationComponent(route, apiService, headerService, adminService, rootService, router);
  });
  it('#ngOnInit', () => {
    spyOn(fixture, 'setUp');
    fixture.ngOnInit();
    expect(fixture.setUp).toHaveBeenCalled();
  });
  it('#setUp', () => {
    fixture.setUp();
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
    expect(apiService.getEvent).toHaveBeenCalledWith(event.EventId);
    expect(adminService.getGradeGroups).toHaveBeenCalled();
    expect(headerService.announceEvent).toHaveBeenCalledWith(event);
  });
  // it('#updateNumberOfKids', () => {
  // });
  // it('#onSubmit', () => {
  // });

});
