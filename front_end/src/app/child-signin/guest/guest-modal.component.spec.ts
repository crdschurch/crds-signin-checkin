import { GuestModalComponent } from './guest-modal.component';
import { ApiService } from '../../shared/services';

let fixture: GuestModalComponent;
let apiService: ApiService;
let fakeModal = { show: {}, hide: {} };

describe('GuestModalComponent', () => {
  beforeEach(() => {
    // var eventId: number = 1234567;
    // var eventTypeId: number = 20;
    fixture = new GuestModalComponent(apiService);
    spyOn(fakeModal, 'show').and.callFake(() => {});
    spyOn(fakeModal, 'hide').and.callFake(() => {});
  });
  describe('#openNewGuestModal', () => {
    let fakeModal = { show: {} };
    beforeEach(() => {
      spyOn(fakeModal, 'show').and.callFake(() => {});
    });
    it('creates new guest', () => {
      fixture.showChildModal(fakeModal);
      expect(fixture.newGuestChild).toBeDefined();
      expect(fixture.newGuestChild.GuestSignin).toBeTruthy();
      expect(fixture.newGuestChild.Selected).toBeTruthy();
      expect(fixture.newGuestChild.DateOfBirth).toBeUndefined();
    });
  });
  // describe('checkEventType', () => {
  //   fixture.eventTypeId = 243;
  //   it('returnsTrueForSMTypes', () => {
  //     expect(fixture.checkSMEventTypeId()).toBeTruthy;
  //   });
  //   fixture.eventTypeId = 20;
  //   it('returnsFalseForKCTypes', () => {
  //     expect(fixture.checkSMEventTypeId()).toBeFalsy;
  //   });
  // });
});
