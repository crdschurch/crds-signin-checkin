import { AvailableChildrenComponent } from './available-children.component';
import { Router } from '@angular/router';
import { ApiService, RootService } from '../../shared/services';
import { ChildSigninService } from '../child-signin.service';
import { Observable } from 'rxjs/Observable';
import { Child, DateOfBirth, Guest, EventParticipants, Event } from '../../shared/models';
import { Constants } from '../../shared/constants';


import * as moment from 'moment';

let fixture: AvailableChildrenComponent;
let apiService: ApiService;
let mockPhoneNumber = '4445556666';
let routerStub: any = {
  params: Observable.of({phoneNumber: mockPhoneNumber})
};
let router: Router;
let rootService: RootService;
let childSigninService: ChildSigninService;

describe('AvailableChildrenComponent', () => {
  beforeEach(() => {
    apiService = jasmine.createSpyObj<ApiService>('apiService', ['getGradeGroups']);
    childSigninService = jasmine.createSpyObj<ChildSigninService>('childSigninService', ['getChildrenByPhoneNumber', 'signInChildren']);
    rootService = jasmine.createSpyObj<RootService>('rootService', ['announceEvent']);
    fixture = new AvailableChildrenComponent(childSigninService, routerStub, router, apiService, rootService);
    fixture.servingOneHour = true;
  });
  describe('#toggleServingHours', () => {
    it('should activate a step and deactivate the other', () => {
      fixture.toggleServingHours(null, 2);
      expect(fixture.isServingOneHour).toBeFalsy();
      expect(fixture.isServingTwoHours).toBeTruthy();
      expect(fixture.numberEventsAttending).toEqual(2);
    });
    it('should allow you to toggle off an option', () => {
      fixture.toggleServingHours(null, 1);
      expect(fixture.isServingOneHour).toBeFalsy();
      expect(fixture.isServingTwoHours).toBeFalsy();
      expect(fixture.numberEventsAttending).toEqual(0);
    });
  });
  describe('#ngOnInit', () => {
    it('should get children and get grade groups for modal', () => {
      spyOn(fixture, 'getChildren').and.callFake(() => {});
      spyOn(fixture, 'populateGradeGroups').and.callFake(() => {});
      fixture.ngOnInit();
      expect(fixture.getChildren).toHaveBeenCalledWith(mockPhoneNumber);
      expect(fixture.populateGradeGroups).toHaveBeenCalled();
    });
  });
  describe('#openNewGuestModal', () => {
    let fakeModal = { show: {} };
    beforeEach(() => {
      spyOn(fakeModal, 'show').and.callFake(() => {});
    });
    it('creates new guest', () => {
      fixture.openNewGuestModal(fakeModal);
      expect(fixture.newGuestChild).toBeDefined();
      expect(fixture.newGuestChild.GuestSignin).toBeTruthy();
      expect(fixture.newGuestChild.Selected).toBeTruthy();
      expect(fixture.newGuestChild.DateOfBirth).toBeUndefined();
    });
  });
  describe('#signIn', () => {
    it('should only send selected kids', () => {
      fixture.eventParticipants = new EventParticipants();
      fixture.eventParticipants.Participants = [new Child(), new Child()];
      fixture.eventParticipants.Participants[0].Selected = false;
      fixture.eventParticipants.Participants[1].Selected = true;
      // spyOn(childSigninService, 'signInChildren').and.callFake(() => {});
      (<jasmine.Spy>(childSigninService.signInChildren)).and.returnValue(Observable.of());
      fixture.signIn();

      // remove unselected participant to make sure it is not sent with call
      fixture.eventParticipants.Participants.splice(0, 1);
      expect(childSigninService.signInChildren).toHaveBeenCalledWith(fixture.eventParticipants, 1);
    });
  });
  describe('#saveNewGuestModal adds new guest to event participants', () => {
    let fakeModal = { show: {}, hide: {} };
    beforeEach(() => {
      spyOn(fakeModal, 'show').and.callFake(() => {});
      spyOn(fakeModal, 'hide').and.callFake(() => {});
    });
    it('creates new guest if valid form', () => {
      let validGuest: Guest = new Guest();
      validGuest.FirstName = 'Pacman';
      validGuest.LastName = 'Jones';
      fixture.newGuestChild = validGuest;
      fixture.guestDOB = new DateOfBirth();
      fixture.guestDOB.month = 4;
      fixture.guestDOB.day = 4;
      fixture.guestDOB.year = 2016;
      // ui event after you pick a date
      fixture.datePickerBlur();
      fixture.saveNewGuest(fakeModal);
      expect(fixture.eventParticipants.Participants[0].FirstName).toEqual('Pacman');
    });
    it('shows error if invalid form', () => {
      let invalidGuest: Guest = new Guest();
      invalidGuest.FirstName = 'Vontaze';
      invalidGuest.LastName = '';
      fixture.newGuestChild = invalidGuest;
      fixture.saveNewGuest(fakeModal);
      expect(fixture.eventParticipants.Participants.length).toEqual(0);
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
      fixture.guestDOB.year = 2015;
      // ui event after you pick a date
      fixture.datePickerBlur();
      fixture.saveNewGuest(fakeModal);
      expect(fixture.eventParticipants.Participants.length).toEqual(0);
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninBadDateOfBirth');
    });
    it('should hide guest and serving for childcare event', () => {
      let eventParticipants: EventParticipants = new EventParticipants();
      let childcareEvent: Event = new Event();
      childcareEvent.EventTypeId = Constants.StudentMinistry6through8;
      eventParticipants.CurrentEvent = childcareEvent;
      fixture.eventParticipants = eventParticipants;
      fixture.setServingAndGuestDisplay();
      expect(fixture.showServingOption).toBe(false);
      expect(fixture.showGuestOption).toBe(false);
    });
    it('should hide guest and serving for childcare event', () => {
      let eventParticipants: EventParticipants = new EventParticipants();
      let childcareEvent: Event = new Event();
      childcareEvent.EventTypeId = Constants.StudentMinistry9through12;
      eventParticipants.CurrentEvent = childcareEvent;
      fixture.eventParticipants = eventParticipants;
      fixture.setServingAndGuestDisplay();
      expect(fixture.showServingOption).toBe(false);
      expect(fixture.showGuestOption).toBe(false);
    });
    it('should show guest and serving for service event', () => {
      let eventParticipants: EventParticipants = new EventParticipants();
      let childcareEvent: Event = new Event();
      childcareEvent.EventTypeId = Constants.StudentMinistry9through12 + 10;
      eventParticipants.CurrentEvent = childcareEvent;
      fixture.eventParticipants = eventParticipants;
      fixture.setServingAndGuestDisplay();
      expect(fixture.showServingOption).toBe(true);
      expect(fixture.showGuestOption).toBe(true);
    });
  });
});
