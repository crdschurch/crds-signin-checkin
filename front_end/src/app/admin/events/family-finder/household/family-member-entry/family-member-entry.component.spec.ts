import { Observable } from 'rxjs';
import { FamilyMemberEntryComponent } from './family-member-entry.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { DateOfBirth, Contact, EventParticipants, Child } from '../../../../../shared/models';
import { Constants } from '../../../../../shared/constants';
import * as moment from 'moment';

const eventId = 4335;
const householdId = 1231;
const participantId = 6542;
let apiService;
let adminService;
let rootService;
let headerService;
let router;
let route;
let eventParticipants = new EventParticipants();
eventParticipants['Participants'] = [new Child()];
eventParticipants.Participants[0].ParticipantId = participantId;
let fixture;
let fakeModal = { show: {}, hide: {} };

describe('FamilyMemberEntryComponent', () => {
  beforeEach(() => {
    apiService = jasmine.createSpyObj('apiService', ['getEvent', 'getGradeGroups']);
    adminService = jasmine.createSpyObj('adminService', ['getChildrenByHousehold', 'findFamilySigninAndPrint',
      'addFamilyMembers', 'updateFamilyMember', 'getHouseholdInformation']);
    rootService = jasmine.createSpyObj('rootService', ['announceEvent']);
    headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
    router = jasmine.createSpyObj<Router>('router', ['navigate']);
    route = new ActivatedRoute();
    route.snapshot = new ActivatedRouteSnapshot();
    route.snapshot.params = {
      eventId: eventId,
      householdId: householdId
    };

    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.getChildrenByHousehold)).and.returnValue(Observable.of(eventParticipants));
    (<jasmine.Spy>(adminService.addFamilyMembers)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.updateFamilyMember)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.getHouseholdInformation)).and.returnValue(Observable.of());
    (<jasmine.Spy>(rootService.announceEvent)).and.returnValue(Observable.of());

    fixture = new FamilyMemberEntryComponent(apiService, adminService, route, headerService, rootService, router);
  });

  describe('#ngOnInit', () => {
    it('should initialize data', () => {
      (<jasmine.Spy>(apiService.getGradeGroups)).and.returnValue(Observable.of());
      fixture.ngOnInit();
      expect(apiService.getEvent).toHaveBeenCalled();
    });
  });

  describe('#saveNewFamilyMember', () => {
    describe('saves new family member to household', () => {
      beforeEach(() => {
        spyOn(fakeModal, 'show').and.callFake(() => {});
        spyOn(fakeModal, 'hide').and.callFake(() => {});
      });
      it('creates new family member if valid form', () => {
        let newContact: Contact = new Contact();
        newContact.Nickname = 'Pacman';
        newContact.LastName = 'Jones';
        newContact.GenderId = 2;
        newContact.IsSpecialNeeds = false;
        newContact.HouseholdId = householdId;
        fixture.contacts = [ newContact ];

        let guestDOB = new DateOfBirth();
        guestDOB.month = 4;
        guestDOB.day = 4;
        guestDOB.year = moment().subtract(1, 'year').year();
        fixture.birthdates = [ guestDOB ];

        fixture.household = householdId;
        // ui event after you pick a date
        fixture.datePickerBlur(newContact, 0);
        let c = fixture.saveNewFamilyMember(fakeModal);
        expect(adminService.addFamilyMembers).toHaveBeenCalledWith([ newContact ], undefined);
      });
    });
  });
  describe('shows error if invalid form', () => {
    it('shows error if invalid not valid name', () => {
      let invalidContact: Contact = new Contact();
      invalidContact.Nickname = 'Pacman';
      invalidContact.LastName = '';
      fixture.contacts = [ invalidContact ];
      let c = fixture.saveNewFamilyMember(fakeModal);
      expect(this.processingaddFamilyMembers).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninAddGuestFormInvalid');
      expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
    });
    it('shows error if bad DOB', () => {
      let invalidContact: Contact = new Contact();
      invalidContact.Nickname = 'Pacman';
      invalidContact.LastName = 'Jones';

      let guestDOB = new DateOfBirth();
      guestDOB.month = 13;
      guestDOB.day = 4;
      guestDOB.year = moment().subtract(1, 'year').year();
      fixture.birthdates = [ guestDOB ];

      fixture.contacts = [ invalidContact ];
      // ui event after you pick a date
      fixture.datePickerBlur(invalidContact, 0);
      let c = fixture.saveNewFamilyMember(fakeModal);
      expect(this.processingaddFamilyMembers).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninBadDateOfBirth');
      expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
    });
    it('shows error if invalid no valid dob', () => {
      let invalidContact: Contact = new Contact();
      invalidContact.Nickname = 'Pacman';
      invalidContact.LastName = 'Jones';
      fixture.contacts = [ invalidContact ];
      fixture.birthdates = [ new DateOfBirth() ];

      // ui event after you pick a date
      fixture.datePickerBlur(invalidContact, 0);
      let c = fixture.saveNewFamilyMember(fakeModal);
      expect(this.processingaddFamilyMembers).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninBadDateOfBirth');
      expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
    });
    it('shows error if invalid no valid group selected', () => {
      let invalidContact: Contact = new Contact();
      invalidContact.Nickname = 'Pacman';
      invalidContact.LastName = 'Jones';

      let guestDOB = new DateOfBirth();
      guestDOB.month = 4;
      guestDOB.day = 4;
      guestDOB.year = moment().subtract(7, 'year').year();
      fixture.birthdates = [ guestDOB ];

      fixture.contacts = [ invalidContact ];
      // ui event after you pick a date
      fixture.datePickerBlur(invalidContact, 0);
      let c = fixture.saveNewFamilyMember(fakeModal);
      expect(this.processingaddFamilyMembers).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckNeedValidGradeSelection');
      expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
    });
    it('shows error if no gender selected', () => {
      let invalidContact: Contact = new Contact();
      invalidContact.Nickname = 'Pacman';
      invalidContact.LastName = 'Jones';
      invalidContact.IsSpecialNeeds = true;

      let guestDOB = new DateOfBirth();
      guestDOB.month = 4;
      guestDOB.day = 4;
      guestDOB.year = moment().subtract(1, 'year').year();
      fixture.birthdates = [ guestDOB ];

      fixture.contacts = [ invalidContact ];
      // ui event after you pick a date
      fixture.datePickerBlur(invalidContact, 0);
      let c = fixture.saveNewFamilyMember(fakeModal);
      expect(this.processingaddFamilyMembers).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckNeedValidGenderSelection');
      expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
    });
    it('shows error if no special needs selected', () => {
      let invalidContact: Contact = new Contact();
      invalidContact.Nickname = 'Pacman';
      invalidContact.LastName = 'Jones';
      invalidContact.GenderId = 1;

      let guestDOB = new DateOfBirth();
      guestDOB.month = 4;
      guestDOB.day = 4;
      guestDOB.year = moment().subtract(1, 'year').year();
      fixture.birthdates = [ guestDOB ];

      fixture.contacts = [ invalidContact ];
      // ui event after you pick a date
      fixture.datePickerBlur(invalidContact, 0);
      let c = fixture.saveNewFamilyMember(fakeModal);
      expect(this.processingaddFamilyMembers).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckNeedSpecialNeedsSelection');
      expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
    });
  });
});
