import { Observable } from 'rxjs';
import { HouseholdComponent } from './household.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { EventParticipants, Child } from '../../../../shared/models';

const eventId = 4335;
const householdId = 1231;
const participantId = 6542;
let apiService = jasmine.createSpyObj('apiService', ['getEvent']);
let adminService = jasmine.createSpyObj('adminService', [, 'getChildrenByHousehold']);
let childSigninService = jasmine.createSpyObj('childSigninService', ['signInChildren']);
let rootService = jasmine.createSpyObj('rootService', ['']);
let headerService = jasmine.createSpyObj('headerService', ['announceEvent']);
let router = jasmine.createSpyObj<Router>('router', ['navigate']);
let route: ActivatedRoute = new ActivatedRoute();
route.snapshot = new ActivatedRouteSnapshot();
route.snapshot.params = {
  eventId: eventId,
  householdId: householdId
};
let eventParticipants = new EventParticipants();
eventParticipants['Participants'] = [new Child()];
eventParticipants.Participants[0].ParticipantId = participantId;
let fixture;

describe('HouseholdComponent', () => {
  beforeEach(() => {
    (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of());
    (<jasmine.Spy>(adminService.getChildrenByHousehold)).and.returnValue(Observable.of(eventParticipants));
    fixture = new HouseholdComponent(apiService, adminService, childSigninService,
      rootService, route, router, headerService);
  });

  describe('#ngOnInit', () => {
    it('should initialize data', () => {
      fixture.ngOnInit();
      expect(fixture.eventParticipants.Participants[0].ParticipantId).toEqual(eventParticipants.Participants[0].ParticipantId);
    });
  });

  describe('#signIn', () => {
    it('should sign in selected kids', () => {
      fixture.eventParticipants = new EventParticipants();
      fixture.eventParticipants.Participants = [new Child(), new Child()];
      fixture.eventParticipants.Participants[0].Selected = false;
      fixture.eventParticipants.Participants[1].Selected = true;
      (<jasmine.Spy>(childSigninService.signInChildren)).and.returnValue(Observable.of());
      fixture.signIn();

      // remove unselected participant to make sure it is not sent with call
      fixture.eventParticipants.Participants.splice(0, 1);
      expect(childSigninService.signInChildren).toHaveBeenCalledWith(fixture.eventParticipants, 1);
    });
  });
});
