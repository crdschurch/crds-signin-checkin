import { Observable } from 'rxjs/Observable';
import { RoomComponent } from './room.component';
import { Room } from '../../../shared/models';

let fixture: RoomComponent;
let adminServiceStub: any = {
  updateRoom(eventId: number, roomId: number, room: Room): any {
    let r = new Room();
    r.Volunteers = 10;
    r.Capacity = 6;
    return Observable.of(r);
  }
};

describe('RoomComponent', () => {

  beforeEach(() => {
    fixture = new RoomComponent(adminServiceStub);
    fixture.room = new Room();
    fixture.room.Volunteers = 10;
    fixture.room.Capacity = 6;
    fixture.room.CheckedIn = 4;
    fixture.room.SignedIn = 20;
    fixture.ngOnInit();
  });

  describe('#constructor', () => {
    it('should intitalize', () => {
      expect(fixture.pending).toBeFalsy();
    });
  });

  describe('#getRoomRatioString', () => {
    it('should return correct ration', () => {
      expect(fixture.getRoomRatioString()).toEqual('4/10');
    });
    it('should return 0 if no CheckedIn and Volunteers', () => {
      fixture.room.Volunteers = 0;
      fixture.room.Capacity = 6;
      fixture.room.CheckedIn = 0;
      fixture.room.SignedIn = 0;
      expect(fixture.getRoomRatioString()).toEqual('0');
    });
  });

  describe('updating a room\'s parameters', () => {
    it('should add one volunteer', () => {
      fixture.add('Volunteers');
      expect(fixture.room.Volunteers).toEqual(11);
    });

    it('should remove four volunteers', () => {
      fixture.remove('Volunteers');
      fixture.remove('Volunteers');
      fixture.remove('Volunteers');
      fixture.remove('Volunteers');
      expect(fixture.room.Volunteers).toEqual(6);
    });

    it('should add two capacitys', () => {
      fixture.add('Capacity');
      fixture.add('Capacity');
      expect(fixture.room.Capacity).toEqual(8);
    });

    it('should remove one capacity', () => {
      fixture.remove('Capacity');
      expect(fixture.room.Capacity).toEqual(5);
    });

    it('should toggle allowed sign-in', () => {
      fixture.toggle('AllowSignIn');
      expect(fixture.room.AllowSignIn).toBeTruthy();
      fixture.toggle('AllowSignIn');
      expect(fixture.room.AllowSignIn).toBeFalsy();
    });

  });

});
