import { Observable } from 'rxjs/Observable';

import { RoomComponent } from './room.component';
import { Child } from '../../shared/models/child';

let fixture: RoomComponent;
let childCheckinServiceStub: any = {
  getChildrenForRoom(roomId: number, eventId: number = null): any {
    let childrenAvailable: Array<Child> = [];

    let child: Child = new Child();
    child.ParticipantId = 123123;
    child.ParticipationStatusId = 3;
    childrenAvailable.push(child);

    child = new Child();
    child.ParticipantId = 12312;
    child.ParticipationStatusId = 4;
    childrenAvailable.push(child);

    child = new Child();
    child.ParticipantId = 123125;
    child.ParticipationStatusId = 4;
    childrenAvailable.push(child);

    return Observable.of(childrenAvailable);
  },
  checkInChildren(child: Child): any {
    child.toggleCheckIn();
    return Observable.of(child);
  }
};

describe('Child Checkin RoomComponent', () => {

  beforeEach(() => {
    fixture = new RoomComponent(childCheckinServiceStub);
    fixture.ngOnInit();
  });

  describe('#constructor', () => {
    it('should intitalize', () => {
      expect(fixture.children.length).toEqual(3);
    });
  });

  describe('#checkedIn', () => {
    it('should only get the checked in children', () => {
      expect(fixture.checkedIn().length).toEqual(2);
      expect(fixture.checkedIn()[0].ParticipantId).toEqual(12312);
    });
  });

  describe('#signedIn', () => {
    it('should only get the signed in children', () => {
      expect(fixture.signedIn().length).toEqual(1);
      expect(fixture.signedIn()[0].ParticipantId).toEqual(123123);
    });
  });

  describe('#toggleCheckIn', () => {
    it('should only get the signed in children', () => {
      fixture.toggleCheckIn(fixture.children[0]);
      expect(fixture.children[0].checkedIn()).toBeTruthy();
    });
  });
});
