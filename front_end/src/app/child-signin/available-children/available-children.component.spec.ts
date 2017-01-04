import { AvailableChildrenComponent } from './available-children.component';
import { Router } from '@angular/router';
import { ApiService, RootService } from '../../shared/services';
import { ChildSigninService } from '../child-signin.service';
import { Observable } from 'rxjs/Observable';
import { Guest } from '../../shared/models';


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

fdescribe('AvailableChildrenComponent', () => {
  beforeEach(() => {
    apiService = jasmine.createSpyObj<ApiService>('apiService', ['getGradeGroups']);
    childSigninService = jasmine.createSpyObj<ChildSigninService>('childSigninService', ['getChildrenByPhoneNumber']);
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
      expect(fixture.newGuestChild.DateOfBirth).toEqual(moment().startOf('day').toDate());
    });
  });
  describe('#saveNewGuestModal adds new guest to event participants', () => {
    let fakeModal = { show: {}, hide: {} };
    beforeEach(() => {
      spyOn(fakeModal, 'show').and.callFake(() => {});
      spyOn(fakeModal, 'hide').and.callFake(() => {});
      fixture.ngOnInit();
    });
    it('creates new guest if valid form', () => {
      let validGuest: Guest = new Guest();
      validGuest.FirstName = 'Pacman';
      validGuest.LastName = 'Jones';
      fixture.newGuestChild = validGuest;
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
  });
});
