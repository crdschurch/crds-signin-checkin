import { Observable } from 'rxjs';
import { FamilyFinderComponent } from './family-finder.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

const event = {
  'EventTitle': 'Cool Event 83',
  'EventId': '92398420398',
};
let router;
let apiService = jasmine.createSpyObj('apiService', ['getEvent', 'getGradeGroups']);
let adminService = jasmine.createSpyObj('adminService', [, 'createNewFamily']);
let headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
let route: ActivatedRoute;
route = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };
let fixture;

describe('FamilyFinderComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
    (<jasmine.Spy>(apiService.getGradeGroups)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.createNewFamily)).and.returnValue(Observable.of());
    fixture = new FamilyFinderComponent(route, apiService, headerService, adminService);
  });
  it('#ngOnInit', () => {
    fixture.ngOnInit();
  });
});
