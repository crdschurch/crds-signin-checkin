import { AssignmentComponent } from './';

import { Router } from '@angular/router';
import { Child, EventParticipants } from '../../shared/models';
import { ChildSigninService } from '../child-signin.service';
import { Observable, Subscription } from 'rxjs';

describe('AssignmentComponent', () => {
  let fixture: AssignmentComponent;
  let childSigninService: ChildSigninService;
  let router: Router;
  let intervalObservable: Observable<number>;
  let intervalSubscription: Subscription;

  beforeEach(() => {
    intervalObservable = Observable.of(1);
    intervalSubscription = intervalObservable.subscribe();

    spyOn(Observable, 'interval').and.returnValue(intervalObservable);
    spyOn(intervalObservable, 'subscribe').and.returnValue(intervalSubscription);
    spyOn(intervalSubscription, 'unsubscribe').and.callFake(() => {});

    childSigninService = jasmine.createSpyObj<ChildSigninService>(
      'childSigninService', ['getEventParticipantsResults', 'printParticipantLabels']);
    router = jasmine.createSpyObj<Router>('router', ['navigate']);

    fixture = new AssignmentComponent(childSigninService, router);
    fixture['error'] = false;
  });

  describe('#ngOnInit', () => {
    it('should set error if no results found', () => {
      (<jasmine.Spy>childSigninService.getEventParticipantsResults).and.returnValue(undefined);
      fixture.ngOnInit();
      expect(childSigninService.getEventParticipantsResults).toHaveBeenCalled();
      expect(Observable.interval).not.toHaveBeenCalled();
      expect(fixture['error']).toBeTruthy();
    });

    describe('with valid results', () => {
      let results: EventParticipants;
      beforeEach(() => {
        results = new EventParticipants();
        results.Participants = [
          new Child(),
          new Child()
        ];
        (<jasmine.Spy>childSigninService.getEventParticipantsResults).and.returnValue(results);
      });

      afterEach(() => {
        expect(Observable.interval).toHaveBeenCalled();
        expect(intervalObservable.subscribe).toHaveBeenCalled();
        expect(intervalSubscription.unsubscribe).toHaveBeenCalled();
        expect(fixture['childrenResult']).toEqual(results.Participants);
      });

      it('should set error if there is a problem printing labels', () => {
        (<jasmine.Spy>childSigninService.printParticipantLabels).and.returnValue(Observable.throw({}));
        fixture.ngOnInit();
        expect(childSigninService.printParticipantLabels).toHaveBeenCalledWith(results);
        expect(router.navigate).not.toHaveBeenCalled();
        expect(fixture['error']).toBeTruthy();
      });

      it('should navigate back to search page when labels successfully print', () => {
        (<jasmine.Spy>childSigninService.printParticipantLabels).and.returnValue(Observable.of(results));
        fixture.ngOnInit();
        expect(childSigninService.printParticipantLabels).toHaveBeenCalledWith(results);
        expect(router.navigate).toHaveBeenCalledWith(['/child-signin/search']);
      });
    });
  });
});
