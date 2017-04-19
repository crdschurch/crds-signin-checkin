import { Observable } from 'rxjs';
import { HouseholdEditComponent } from './household-edit.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Household, State, Country } from '../../../../../shared/models';

const eventId = 4335;
const householdId = 1231;
const participantId = 6542;

let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let adminService = jasmine.createSpyObj('adminService', ['getHouseholdInformation', 'getStates', 'getCountries', 'updateHousehold']);
let rootService = jasmine.createSpyObj('rootService', ['announceEvent']);
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

let states: Array<State> = [];
states.push(new State());
states[0].StateId = 123;

let countries: Array<Country> = [];
countries.push(new Country());
countries[0].CountryId = 123;

let fixture;

describe('HouseholdComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.getHouseholdInformation)).and.returnValue(Observable.of(household));
    (<jasmine.Spy>(adminService.updateHousehold)).and.returnValue(Observable.of(household));
    (<jasmine.Spy>(adminService.getStates)).and.returnValue(Observable.of(states));
    (<jasmine.Spy>(adminService.getCountries)).and.returnValue(Observable.of(countries));
    fixture = new HouseholdEditComponent(apiService, adminService, route, headerService, rootService, router);
  });

  describe('#ngOnInit', () => {
    it('should initialize data', () => {
      fixture.ngOnInit();
      expect(fixture.household.HouseholdName).toEqual(household.HouseholdName);
      expect(fixture.states[0].StateId).toEqual(states[0].StateId);
      expect(fixture.countries[0].CountryId).toEqual(countries[0].CountryId);
    });
  });

  describe('#onSubmit', () => {
    it('should submit data', () => {
      let form = jasmine.createSpyObj('form', ['valid', 'dirty']);
      (<jasmine.Spy>(form.valid)).and.returnValue(true);
      (<jasmine.Spy>(form.dirty)).and.returnValue(true);
      fixture.onSubmit(form);
      expect(fixture.adminService.updateHousehold).toHaveBeenCalledWith(fixture.household);
    });
  });

  describe('#onSave', () => {
    it('should save data', () => {
      let form = jasmine.createSpyObj('form', ['valid', 'dirty']);
      (<jasmine.Spy>(form.valid)).and.returnValue(true);
      (<jasmine.Spy>(form.dirty)).and.returnValue(true);
      fixture.onSave(form);
      expect(fixture.adminService.updateHousehold).toHaveBeenCalledWith(fixture.household);
    });

    it('should not save data', () => {
      let form = jasmine.createSpyObj('form', ['valid', 'dirty']);
      (<jasmine.Spy>(form.valid)).and.returnValue(false);
      (<jasmine.Spy>(form.dirty)).and.returnValue(true);
      fixture.onSave(form);
      expect(fixture.router.navigate).toHaveBeenCalled;
    });
  });
});
