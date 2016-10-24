import { RoomComponent } from './room.component';
import { Room } from './room';

let fixture: RoomComponent;
let adminServiceStub: any = {};

describe('RoomComponent', () => {

  beforeEach(() => {
    fixture = new RoomComponent(adminServiceStub);
    fixture.room = new Room();
    fixture.room.Volunteers = 10;
    fixture.room.Capacity = 6;
    fixture.ngOnInit();
  });

  describe('#constructor', () => {
    it('should intitalize', () => {
      expect(fixture.pending).toBeFalsy();
    });
  });

  describe('updating a room\'s parameters', () => {
    it('should add one volunteer', () => {
      fixture.add("Volunteers");
      expect(fixture.room.Volunteers).toEqual(11);
    });

    it('should remove four volunteers', () => {
      fixture.remove("Volunteers");
      fixture.remove("Volunteers");
      fixture.remove("Volunteers");
      fixture.remove("Volunteers");
      expect(fixture.room.Volunteers).toEqual(6);
    });

    it('should add two capacitys', () => {
      fixture.add("Capacity");
      fixture.add("Capacity");
      expect(fixture.room.Capacity).toEqual(8);
    });

    it('should remove one capacity', () => {
      fixture.remove("Capacity");
      expect(fixture.room.Capacity).toEqual(5);
    });

    it('should toggle allowed sign-in', () => {
      fixture.toggle("AllowSignIn");
      expect(fixture.room.AllowSignIn).toBeTruthy();
      fixture.toggle("AllowSignIn");
      expect(fixture.room.AllowSignIn).toBeFalsy();
    });

  });

})
