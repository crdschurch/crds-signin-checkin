import { Observable } from 'rxjs';
import { FamilyFinderComponent } from './family-finder.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

const event = {
  'EventTitle': 'Cool Event 83',
  'EventId': '92398420398',
};
let router;
let adminService = jasmine.createSpyObj('adminService', [, 'createNewFamily']);
let route: ActivatedRoute = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };
let fixture;

describe('FamilyFinderComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(adminService.createNewFamily)).and.returnValue(Observable.of());
    fixture = new FamilyFinderComponent(adminService, router);
  });
  it('#ngOnInit', () => {
    fixture.ngOnInit();
  });
});
