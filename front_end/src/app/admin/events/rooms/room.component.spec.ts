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

let zone: any = {
  run(): any {
    return Observable.of();
  }
};
let channelService: any = {
  sub(): any {
    return Observable.of();
  }
};
let route: any = {
  snapshot: {
    params: { 'eventId': 123 },
  }
};
let rootService: any;

describe('RoomComponent', () => {
  beforeEach(() => {
    fixture = new RoomComponent(route, adminServiceStub, rootService, channelService, zone);
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

  describe('capacity alerts', () => {
    describe('#isCapacityWarning', () => {
      it('should return true if checked in + signed in is >= 80% of capacity', () => {
        fixture.room.Capacity = 6;
        fixture.room.CheckedIn = 3;
        fixture.room.SignedIn = 2;
        expect(fixture.isCapacityWarning()).toBeTruthy();
        expect(fixture.isCapacityDanger()).toBeFalsy();
        fixture.room.Capacity = 20;
        fixture.room.CheckedIn = 15;
        fixture.room.SignedIn = 1;
        expect(fixture.isCapacityWarning()).toBeTruthy();
        expect(fixture.isCapacityDanger()).toBeFalsy();
      });
    });
    describe('#isCapacityDanger', () => {
      it('should return true if checked in + signed in >= 100% capacity', () => {
        fixture.room.Capacity = 5;
        fixture.room.CheckedIn = 3;
        fixture.room.SignedIn = 2;
        expect(fixture.isCapacityWarning()).toBeFalsy();
        expect(fixture.isCapacityDanger()).toBeTruthy();
      });
    });
    it('should return no warnings if < 80%', () => {
      fixture.room.Capacity = 100;
      fixture.room.CheckedIn = 70;
      fixture.room.SignedIn = 9;
      expect(fixture.isCapacityWarning()).toBeFalsy();
      expect(fixture.isCapacityDanger()).toBeFalsy();
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

  describe('#ageRangeAndGrades', () => {
    it('should return an Array with Add', () => {
      expect(fixture.ageRangeAndGrades()).toEqual(['Add']);
    });
  });

  describe('updating a room\'s parameters', () => {
    it('should add one volunteer', () => {
      fixture.add('Volunteers');
      expect(fixture.room.Volunteers).toEqual(11);
      expect(fixture.dirty).toEqual(true);
    });

    it('should remove four volunteers', () => {
      fixture.remove('Volunteers');
      fixture.remove('Volunteers');
      fixture.remove('Volunteers');
      fixture.remove('Volunteers');
      expect(fixture.room.Volunteers).toEqual(6);
      expect(fixture.dirty).toEqual(true);
    });

    it('should add two capacitys', () => {
      fixture.add('Capacity');
      fixture.add('Capacity');
      expect(fixture.room.Capacity).toEqual(8);
      expect(fixture.dirty).toEqual(true);
    });

    it('should remove one capacity', () => {
      fixture.remove('Capacity');
      expect(fixture.room.Capacity).toEqual(5);
      expect(fixture.dirty).toEqual(true);
    });

    it('should toggle allowed sign-in', () => {
      fixture.toggle('AllowSignIn');
      expect(fixture.room.AllowSignIn).toBeTruthy();
      fixture.toggle('AllowSignIn');
      expect(fixture.room.AllowSignIn).toBeFalsy();
      expect(fixture.dirty).toEqual(true);
    });

    it('should save', () => {
      fixture.dirty = true;
      fixture.saveRoom();
      expect(fixture.dirty).toEqual(false);
    });

  });

});
