import { AlternateRoomsComponent } from './alternate-rooms.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { Room } from '../../../shared/models';
import { Observable } from 'rxjs';

fdescribe('AlternateRoomsComponent', () => {
  let fixture: AlternateRoomsComponent;
  let adminService = jasmine.createSpyObj('adminService', ['getBumpingRooms']);
  let route: ActivatedRoute;
  route = new ActivatedRoute();
  route.snapshot = new ActivatedRouteSnapshot();
  route.snapshot.params = {
    eventId: '123',
    roomId: '456'
  };

  beforeEach(() => {
    fixture = new AlternateRoomsComponent(adminService, route);
    (<jasmine.Spy>(adminService.getBumpingRooms)).and.returnValue(Observable.of([{}]));
  });

  describe('#ngOnInit', () => {
    it('should call adminService', () => {
      fixture.ngOnInit();
      expect(adminService.getBumpingRooms).toHaveBeenCalledWith(route.snapshot.params['eventId'], route.snapshot.params['roomId']);
    });
  });


});
