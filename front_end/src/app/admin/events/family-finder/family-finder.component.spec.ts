import { Observable } from 'rxjs';
import { FamilyFinderComponent } from './family-finder.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

const event = {
  'EventTitle': 'Cool Event 83',
  'EventId': '92398420398',
};

let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let adminService = jasmine.createSpyObj('adminService', [, 'findFamilies']);
let headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
let route: ActivatedRoute = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };
let fixture;

describe('FamilyFinderComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
    (<jasmine.Spy>(adminService.findFamilies)).and.returnValue(Observable.of());
    fixture = new FamilyFinderComponent(route, adminService, headerService, apiService);
  });

  it('#ngOnInit', () => {
    fixture.ngOnInit();
    expect(fixture.processing).toBeFalsy;
    expect(fixture.searched).toBeFalsy;
    expect(apiService.getEvent).toHaveBeenCalledWith(event.EventId);
    expect(headerService.announceEvent).toHaveBeenCalledWith(event);
  });

  it('#setSearchValue(search)', () => {
    fixture.setSearchValue('hi');
    expect(fixture.search).toEqual('hi');
  });

  it('#onClearSearch(box)', () => {
    fixture.search = 'hi';
    expect(fixture.search).toEqual('hi');

    fixture.onClearSearch({value: 'hello'});
    expect(fixture.search).toEqual('');
    expect(fixture.contacts).toEqual([]);
    expect(fixture.searched).toBeFalsy;
  });

  it('#onSearch()', () => {
    fixture.onSearch();
    expect(fixture.processing).toBeTruthy;
    expect(fixture.searched).toBeTruthy;
    expect(adminService.findFamilies).toHaveBeenCalledWith(fixture.search);
  });
});
