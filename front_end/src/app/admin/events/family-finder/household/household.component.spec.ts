import { Observable } from 'rxjs';
import { HouseholdComponent } from './household.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';

const eventId = 4335;
const householdId = 1231;
let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let adminService = jasmine.createSpyObj('adminService', [, 'findFamilies']);
let childSigninService = jasmine.createSpyObj('childSigninService', ['']);
let rootService = jasmine.createSpyObj('rootService', ['']);
let headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
let router = jasmine.createSpyObj<Router>('router', ['navigate']);
let route: ActivatedRoute = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = {
  eventId: eventId,
  householdId: householdId
};
let fixture;

describe('HouseholdComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.findFamilies)).and.returnValue(Observable.of());
    fixture = new HouseholdComponent(apiService, adminService, childSigninService,
      rootService, route, router, headerService);
  });

  it('#ngOnInit', () => {
    fixture.ngOnInit();
    fail();
  });

  it('#signIn()', () => {
    fixture.signIn();
    fail();
  });
});
