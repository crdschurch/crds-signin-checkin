import { Observable } from 'rxjs';
import { NewFamilyRegistrationComponent } from './new-family-registration.component';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { NewFamily, NewParent, NewChild } from '../../../../shared/models';
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
let route: ActivatedRoute;
route = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };

fdescribe('NewFamilyRegistrationComponent', () => {
  it('#ngOnInit', () => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
    (<jasmine.Spy>(adminService.getGradeGroups)).and.returnValue(Observable.of());
    let fixture = new NewFamilyRegistrationComponent(route, apiService, headerService, adminService, router);
    fixture.ngOnInit();
    expect(apiService.getEvent).toHaveBeenCalledWith(event.EventId);
    expect(adminService.getGradeGroups).toHaveBeenCalledWith(event.EventId);
    expect(headerService.announceEvent).toHaveBeenCalledWith(event);
  });

});
