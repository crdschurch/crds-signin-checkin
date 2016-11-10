import { RoomBumpComponent } from './room-bump.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { Room } from '../../../shared/models';

describe('RoomBumpComponent', () => {
  let fixture: RoomBumpComponent;
  let adminService = jasmine.createSpyObj('adminService', ['getBumpingRooms']);
  let route: ActivatedRoute;
  route = new ActivatedRoute();
  route.snapshot = new ActivatedRouteSnapshot();
  route.snapshot.params = {
    eventId: '123',
    roomId: '456'
  };

  describe('available rooms', () => {
    beforeEach(() => {
      fixture = new RoomBumpComponent(adminService, route);
      fixture.index = 1;
      let availableRoom = Room.fromJson({});
      fixture.room = availableRoom;
      fixture.bumpingRooms = [
        Room.fromJson({BumpingRulePriority: 0, EventRoomId: 555}),
        Room.fromJson({BumpingRulePriority: 1, EventRoomId: 777})
      ];
    });

    describe('#bump', () => {
      it('should move an available room to the bottom of the bumping list and send call to update backend', () => {
        fixture.bump();
        expect(fixture.room.BumpingRulePriority).toBeDefined();
        expect(fixture.room.BumpingRulePriority).toEqual(2);
        expect(adminService.updateBumpingRooms).toHaveBeenCalled();
      });
    });
  });


  describe('#bump', () => {
    beforeEach(() => {
      fixture = new RoomBumpComponent(adminService, route);
      fixture.index = 1;
      fixture.room = Room.fromJson({BumpingRulePriority: 2, EventRoomId: 666});
      fixture.bumpingRooms = [
        Room.fromJson({BumpingRulePriority: 1, EventRoomId: 555}),
        fixture.room,
        Room.fromJson({BumpingRulePriority: 3, EventRoomId: 777})
      ];
    });

    describe('#bumpUp', () => {
      it('should move a room up the bumping list and send call to update backend', () => {
        fixture.bumpUp();
        expect(fixture.room.BumpingRulePriority).toEqual(1);
        expect(adminService.updateBumpingRooms).toHaveBeenCalled();
      });
    });

    describe('#bumpDown', () => {
      it('should move a room down the bumping list and send call to update backend', () => {
        fixture.bumpDown();
        expect(fixture.room.BumpingRulePriority).toEqual(3);
        expect(adminService.updateBumpingRooms).toHaveBeenCalled();
      });
    });

    describe('#unBump', () => {
      it('should move a bumped room to available rooms and send call to update backend', () => {
        fixture.unBump();
        expect(fixture.room.BumpingRulePriority).not.toBeDefined();
        expect(adminService.updateBumpingRooms).toHaveBeenCalled();
      });
    });
  });

});
