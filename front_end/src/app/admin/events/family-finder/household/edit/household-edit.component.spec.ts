import { Observable } from 'rxjs';
import { HouseholdEditComponent } from './household-edit.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Household, State, Country } from '../../../../../shared/models';

const eventId = 4335;
const householdId = 1231;
const participantId = 6542;

let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let adminService = jasmine.createSpyObj('adminService', ['getHouseholdInformation']);
let headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
let router = jasmine.createSpyObj<Router>('router', ['navigate']);
let route: ActivatedRoute = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = {
  eventId: eventId,
  householdId: householdId
};

let household = new Household();
household.HouseholdName = 'test';

let fixture;

describe('HouseholdComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.getHouseholdInformation)).and.returnValue(Observable.of(household));
    fixture = new HouseholdEditComponent(apiService, adminService, route, headerService);
  });

  describe('#ngOnInit', () => {
    it('should initialize data', () => {
      fixture.ngOnInit();
      expect(fixture.household.HouseholdName).toEqual(household.HouseholdName);
    });
  });
});
