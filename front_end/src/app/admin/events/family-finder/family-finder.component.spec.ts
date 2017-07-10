import { Observable } from 'rxjs';
import { FamilyFinderComponent } from './family-finder.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';

const event = {
  'EventTitle': 'Cool Event 83',
  'EventId': '92398420398',
};

let adminService = jasmine.createSpyObj('adminService', [, 'findFamilies']);
let route: ActivatedRoute = new ActivatedRoute();
let router = jasmine.createSpyObj('router', ['navigate']);
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = { eventId: event.EventId };
route.snapshot.queryParams = { };
let fixture;

describe('FamilyFinderComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(adminService.findFamilies)).and.returnValue(Observable.of());
    (<jasmine.Spy>(router.navigate)).and.returnValue(Observable.of());
    fixture = new FamilyFinderComponent(route, adminService, router);
  });

  it('#ngOnInit', () => {
    fixture.ngOnInit();
    expect(fixture.processing).toBeFalsy;
    expect(fixture.searched).toBeFalsy;
  });

  it('#ngOnInit', () => {
    route.snapshot.queryParams = { search: 'test' };
    fixture.ngOnInit();
    expect(fixture.processing).toBeFalsy;
    expect(fixture.searched).toBeFalsy;
    expect(fixture.search).toEqual('test');
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

  describe('#getSearchParams()', () => {
    it('should populate phone', () => {
      const phone = '513-345-4555';
      fixture.search = phone;
      expect(fixture.getSearchParams().phone).toEqual(phone);
    });
    it('should populate email', () => {
      const email = 'edmond@xavier.edu';
      fixture.search = email;
      expect(fixture.getSearchParams().email).toEqual(email);
    });
    it('should populate last name', () => {
      const last = 'sumner';
      fixture.search = last;
      expect(fixture.getSearchParams().last).toEqual(last);
    });
    it('should populate name', () => {
      const last = 'sumner, edmond';
      fixture.search = last;
      expect(fixture.getSearchParams().last).toEqual('sumner');
      expect(fixture.getSearchParams().first).toEqual('edmond');
    });
  });
});
