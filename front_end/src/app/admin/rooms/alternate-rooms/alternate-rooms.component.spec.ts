import { AlternateRoomsComponent } from './alternate-rooms.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { Room } from '../../../shared/models';
import { Observable } from 'rxjs';

describe('AlternateRoomsComponent', () => {
  let fixture: AlternateRoomsComponent;
  let adminService = jasmine.createSpyObj('adminService', ['getBumpingRooms']);
  let route: ActivatedRoute;
  route = new ActivatedRoute();
  route.snapshot = new ActivatedRouteSnapshot();
  route.snapshot.params = { eventId: '123', roomId: '456' };

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

  describe('sorting rooms', () => {
    const roomUnbumped1 = Room.fromJson({ RoomId: 111 });
    const roomUnbumped2 = Room.fromJson({ RoomId: 222 });
    const roomBumped2 = Room.fromJson({ RoomId: 333, BumpingRulePriority: 2 });
    const roomBumped0 = Room.fromJson({ RoomId: 444, BumpingRulePriority: 0 });
    const roomBumped1 = Room.fromJson({ RoomId: 555, BumpingRulePriority: 1 });
    const allRooms = [roomUnbumped1, roomUnbumped2, roomBumped2, roomBumped0, roomBumped1];

    beforeEach(() => {
      fixture = new AlternateRoomsComponent(adminService, route);
      (<jasmine.Spy>(adminService.getBumpingRooms)).and.returnValue(Observable.of(allRooms));
    });

    describe('#availableRooms()', () => {
      it('get only available rooms', () => {
        fixture.ngOnInit();
        expect(fixture.availableRooms.length).toEqual(2);
      });
    });

    describe('#bumpingRooms()', () => {
      it('get only bumping rooms, sort by priority', () => {
        fixture.ngOnInit();
        expect(fixture.bumpingRooms.length).toEqual(3);
        expect(fixture.bumpingRooms[0].RoomId).toEqual(roomBumped0.RoomId);
        expect(fixture.bumpingRooms[1].RoomId).toEqual(roomBumped1.RoomId);
        expect(fixture.bumpingRooms[2].RoomId).toEqual(roomBumped2.RoomId);
      });
    });
  });
});
