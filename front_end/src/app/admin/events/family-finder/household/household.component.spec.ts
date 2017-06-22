import { Observable } from 'rxjs';
import { HouseholdComponent } from './household.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { DateOfBirth, Contact, EventParticipants, Child } from '../../../../shared/models';
import { Constants } from '../../../../shared/constants';
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

describe('HouseholdComponent', () => {
  beforeEach(() => {
    apiService = jasmine.createSpyObj('apiService', ['getEvent', 'getGradeGroups']);
    adminService = jasmine.createSpyObj('adminService', ['getChildrenByHousehold', 'findFamilySigninAndPrint',
      'addFamilyMembers', 'updateFamilyMember']);
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
    (<jasmine.Spy>(rootService.announceEvent)).and.returnValue(Observable.of());

    fixture = new HouseholdComponent(apiService, adminService, rootService, route, router, headerService);
  });

  describe('#ngOnInit', () => {
    it('should initialize data', () => {
      (<jasmine.Spy>(apiService.getGradeGroups)).and.returnValue(Observable.of());
      fixture.ngOnInit();
      expect(apiService.getEvent).toHaveBeenCalled();
    });
  });

  describe('#signIn', () => {
    it('should sign in selected kids', () => {
      fixture.setServingHours(1);
      fixture.eventParticipants = new EventParticipants();
      fixture.eventParticipants.Participants = [new Child(), new Child()];
      fixture.eventParticipants.Participants[0].Selected = false;
      fixture.eventParticipants.Participants[1].Selected = true;
      (<jasmine.Spy>(adminService.findFamilySigninAndPrint)).and.returnValue(Observable.of());
      fixture.signIn();

      // remove unselected participant to make sure it is not sent with call
      fixture.eventParticipants.Participants.splice(0, 1);
      expect(adminService.findFamilySigninAndPrint).toHaveBeenCalledWith(fixture.eventParticipants, 1);
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
        fixture.contact = newContact;
        fixture.guestDOB = new DateOfBirth();
        fixture.guestDOB.month = 4;
        fixture.guestDOB.day = 4;
        fixture.guestDOB.year = moment().subtract(1, 'year').year();
        fixture.household = householdId;
        // ui event after you pick a date
        fixture.datePickerBlur();
        let c = fixture.saveNewFamilyMember(fakeModal);
        expect(adminService.addFamilyMembers).toHaveBeenCalledWith([ newContact ], undefined);
      });
    });
    it('edits existing family member if valid form', () => {
      let existingContact: Contact = new Contact();
      existingContact.Nickname = 'Pacman';
      existingContact.LastName = 'Jones';
      existingContact.GenderId = 2;
      existingContact.IsSpecialNeeds = false;
      existingContact.ContactId = 789;
      existingContact.HouseholdId = householdId;
      fixture.contact = existingContact;
      fixture.guestDOB = new DateOfBirth();
      fixture.guestDOB.month = 4;
      fixture.guestDOB.day = 4;
      fixture.guestDOB.year = moment().subtract(1, 'year').year();
      // ui event after you pick a date
      fixture.datePickerBlur();
      let c = fixture.saveNewFamilyMember(fakeModal);
      expect(adminService.updateFamilyMember).toHaveBeenCalledWith(existingContact);
    });
  });
    describe('shows error if invalid form', () => {
      it('shows error if invalid not valid name', () => {
        let invalidContact: Contact = new Contact();
        invalidContact.Nickname = 'Pacman';
        invalidContact.LastName = '';
        fixture.contact = invalidContact;
        let c = fixture.saveNewFamilyMember(fakeModal);
        expect(this.processingaddFamilyMembers).toBeFalsy();
        expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninAddGuestFormInvalid');
        expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
      });
      it('shows error if bad DOB', () => {
        let invalidContact: Contact = new Contact();
        invalidContact.Nickname = 'Pacman';
        invalidContact.LastName = 'Jones';
        fixture.guestDOB = new DateOfBirth();
        fixture.guestDOB.month = 13;
        fixture.guestDOB.day = 4;
        fixture.guestDOB.year = moment().subtract(1, 'year').year();
        fixture.contact = invalidContact;
        // ui event after you pick a date
        fixture.datePickerBlur();
        let c = fixture.saveNewFamilyMember(fakeModal);
        expect(this.processingaddFamilyMembers).toBeFalsy();
        expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninBadDateOfBirth');
        expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
      });
      it('shows error if invalid no valid dob', () => {
        let invalidContact: Contact = new Contact();
        invalidContact.Nickname = 'Pacman';
        invalidContact.LastName = 'Jones';
        fixture.contact = invalidContact;
        // ui event after you pick a date
        fixture.datePickerBlur();
        let c = fixture.saveNewFamilyMember(fakeModal);
        expect(this.processingaddFamilyMembers).toBeFalsy();
        expect(rootService.announceEvent).toHaveBeenCalledWith('echeckChildSigninBadDateOfBirth');
        expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
      });
      it('shows error if invalid no valid group selected', () => {
        let invalidContact: Contact = new Contact();
        invalidContact.Nickname = 'Pacman';
        invalidContact.LastName = 'Jones';
        fixture.guestDOB = new DateOfBirth();
        fixture.guestDOB.month = 12;
        fixture.guestDOB.day = 4;
        fixture.guestDOB.year = moment().subtract(7, 'year').year();
        fixture.contact = invalidContact;
        // ui event after you pick a date
        fixture.datePickerBlur();
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
        fixture.guestDOB = new DateOfBirth();
        fixture.guestDOB.month = 12;
        fixture.guestDOB.day = 4;
        fixture.guestDOB.year = moment().subtract(1, 'year').year();
        fixture.contact = invalidContact;
        // ui event after you pick a date
        fixture.datePickerBlur();
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
        fixture.guestDOB = new DateOfBirth();
        fixture.guestDOB.month = 12;
        fixture.guestDOB.day = 4;
        fixture.guestDOB.year = moment().subtract(1, 'year').year();
        fixture.contact = invalidContact;
        // ui event after you pick a date
        fixture.datePickerBlur();
        let c = fixture.saveNewFamilyMember(fakeModal);
        expect(this.processingaddFamilyMembers).toBeFalsy();
        expect(rootService.announceEvent).toHaveBeenCalledWith('echeckNeedSpecialNeedsSelection');
        expect(adminService.addFamilyMembers).not.toHaveBeenCalled();
      });
    });
  });

  describe('#editMode', () => {
    it('should return true if there is a contact', () => {
      let existingContact: Contact = new Contact();
      existingContact.ContactId = 444444;
      fixture.contact = existingContact;
      expect(fixture.editMode).toBeTruthy();
    });
    it('should return false if new contact', () => {
      let newContact: Contact = new Contact();
      fixture.contact = newContact;
      expect(fixture.editMode).toBeFalsy();
    });
  });

  describe('test', () => {
    it('Childcare Group and Childcare Event Should Be False', () => {
      let child: Child = new Child();
      child.GroupId = 1201029;
      child.canCheckIn(Constants.ChildCareEventType);

      expect(child.CanCheckIn).toBeTruthy();
    });
    it('Childcare Group and SM Event Should Be False', () => {
      let child: Child = new Child();
      child.GroupId = 1201029;
      child.canCheckIn(Constants.StudentMinistry6through8EventType);

      expect(child.CanCheckIn).toBeFalsy();
    });
    it('Kids Club Group and KC Event Should Be True', () => {
      let child: Child = new Child();
      child.GroupId = 1201029;
      child.canCheckIn(Constants.BigEventType + 1000);

      expect(child.CanCheckIn).toBeTruthy();
    });
    it('Kids Club Group and SM Event Should Be False', () => {
      let child: Child = new Child();
      child.GroupId = 1201029;
      child.canCheckIn(Constants.StudentMinistry6through8EventType);

      expect(child.CanCheckIn).toBeFalsy();
    });
    it('SM6-8 Group and SM6-8 Event Should Be True', () => {
      let child: Child = new Child();
      child.GroupId = Constants.MsmSixth;
      child.canCheckIn(Constants.StudentMinistry6through8EventType);

      expect(child.CanCheckIn).toBeTruthy();
    });
    it('SM6-8 Group and SM9-12 Event Should Be False', () => {
      let child: Child = new Child();
      child.GroupId = Constants.MsmSixth;
      child.canCheckIn(Constants.StudentMinistry9through12EventType);

      expect(child.CanCheckIn).toBeFalsy();
    });
    it('SM9-12 Group and SM9-12 Event Should Be True', () => {
      let child: Child = new Child();
      child.GroupId = Constants.HighSchoolEleventh;
      child.canCheckIn(Constants.StudentMinistry9through12EventType);

      expect(child.CanCheckIn).toBeTruthy();
    });
    it('SM9-12 Group and SM6-8 Event Should Be False', () => {
      let child: Child = new Child();
      child.GroupId = Constants.HighSchoolEleventh;
      child.canCheckIn(Constants.StudentMinistry6through8EventType);

      expect(child.CanCheckIn).toBeFalsy();
    });
    it('Big Group and Big Event Should Be True', () => {
      let child: Child = new Child();
      child.GroupId = Constants.HighSchoolEleventh;
      child.canCheckIn(Constants.BigEventType);

      expect(child.CanCheckIn).toBeTruthy();
    });
    it('Big Group and ChildCare Event Should Be False', () => {
      let child: Child = new Child();
      child.GroupId = Constants.HighSchoolEleventh;
      child.canCheckIn(Constants.ChildCareEventType);

      expect(child.CanCheckIn).toBeFalsy();
    });
  });
