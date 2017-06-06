import { GuestModalComponent } from './guest-modal.component';
import { ApiService, RootService } from '../../shared/services';
import { DateOfBirth, EventParticipants, Guest, Group } from '../../shared/models';

import * as moment from 'moment';

let fixture: GuestModalComponent;
let apiService: ApiService;
let rootService: RootService;
let fakeModal = { show: {}, hide: {} };

describe('GuestModalComponent', () => {
  beforeEach(() => {
    rootService = jasmine.createSpyObj<RootService>('rootService', ['announceEvent']);
    spyOn(fakeModal, 'show').and.callFake(() => {});
    spyOn(fakeModal, 'hide').and.callFake(() => {});
    fixture = new GuestModalComponent(apiService, rootService);
  });
  describe('#openNewGuestModal', () => {
    let fakeModal = { show: {}, hide: {} };
    beforeEach(() => {
      spyOn(fakeModal, 'show').and.callFake(() => {});
      spyOn(fakeModal, 'hide').and.callFake(() => {});
    });
    it('creates new guest', () => {
      fixture.showChildModal(fakeModal);
      expect(fixture.newGuestChild).toBeDefined();
      expect(fixture.newGuestChild.GuestSignin).toBeTruthy();
      expect(fixture.newGuestChild.Selected).toBeTruthy();
      expect(fixture.newGuestChild.DateOfBirth).toBeUndefined();
    });
    it('creates new guest if valid form', () => {
      let validGuest: Guest = new Guest();
      validGuest.FirstName = 'Pacman';
      validGuest.LastName = 'Jones';
      fixture.newGuestChild = validGuest;
      fixture.guestDOB = new DateOfBirth();
      fixture.guestDOB.month = 4;
      fixture.guestDOB.day = 4;
      fixture.guestDOB.year = moment().subtract(1, 'year').year();
      // ui event after you pick a date
      fixture.datePickerBlur();
      fixture.saveNewGuest(fakeModal);
      expect(fixture.newGuestChild.FirstName).toEqual('Pacman');
    });
    it('shows error if invalid form', () => {
      let invalidGuest: Guest = new Guest();
      invalidGuest.FirstName = 'Vontaze';
      invalidGuest.LastName = '';
      fixture.newGuestChild = invalidGuest;
      console.log(fixture.newGuestChild.LastName);
      fixture.saveNewGuest(fakeModal);
      console.log('root service: ' + rootService);
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninAddGuestFormInvalid');
    });
    it('shows dob error if invalid date', () => {
      let validGuest: Guest = new Guest();
      validGuest.FirstName = 'Vontaze';
      validGuest.LastName = 'Burfict';
      fixture.newGuestChild = validGuest;
      fixture.guestDOB = new DateOfBirth();
      fixture.guestDOB.month = 2;
      fixture.guestDOB.day = 30;
      fixture.guestDOB.year = moment().subtract(2, 'year').year();
      // ui event after you pick a date
      fixture.datePickerBlur();
      fixture.saveNewGuest(fakeModal);
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninBadDateOfBirth');
    });
  });
  describe('checkEventType', () => {
    it('returnsTrueForSMTypes', () => {
      fixture.eventTypeId = 243;
      expect(fixture.checkSMEventTypeId()).toBeTruthy;
    });
    it('returnsFalseForKCTypes', () => {
      fixture.eventTypeId = 20;
      expect(fixture.checkSMEventTypeId()).toBeFalsy;
    });
  });
  describe('#ngOnInit', () => {
    it('should get children and get grade groups for modal', () => {
      spyOn(fixture, 'populateGradeGroups').and.callFake(() => {});
      fixture.ngOnInit();
      expect(fixture.populateGradeGroups).toHaveBeenCalled();
    });
  });
});
