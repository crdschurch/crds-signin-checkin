import { UserService, LoginRedirectService } from './';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { Router } from '@angular/router';
import { User } from '../models';
import { Observable, Subscription } from 'rxjs';
import * as moment from 'moment';

describe('UserService', () => {
  let fixture: UserService;
  let loginRedirectService: LoginRedirectService;
  let cookieService: CookieService;
  let router: Router;

  beforeEach(() => {
    loginRedirectService = jasmine.createSpyObj<LoginRedirectService>('loginRedirectService', ['redirectToLogin']);
    cookieService = jasmine.createSpyObj<CookieService>('cookieService', ['getObject', 'putObject']);
    router = <Router>{
      routerState: {
        snapshot: {
          url: '/current/page'
        }
      }
    };

    fixture = new UserService(cookieService, loginRedirectService, router);
  });

  describe('#getUser', () => {
    it('should set new user cookie if none exists yet', () => {
      let newCookie = new User();
      newCookie.token = '123';
      (<jasmine.Spy>cookieService.getObject).and.returnValues(undefined, newCookie);
      let user = fixture.getUser();
      expect(cookieService.getObject).toHaveBeenCalledTimes(2);
      expect(cookieService.putObject).toHaveBeenCalledWith('user', new User());
      expect(user).toEqual(newCookie);
    });

    it('should get existing user cookie if it exists', () => {
      let existingCookie = new User();
      existingCookie.token = '123';
      (<jasmine.Spy>cookieService.getObject).and.returnValue(existingCookie);
      let user = fixture.getUser();
      expect(cookieService.putObject).not.toHaveBeenCalled();
      expect(cookieService.getObject).toHaveBeenCalledTimes(2);
      expect(user).toEqual(existingCookie);
    });
  });

  describe('#setUser', () => {
    let originalObservableTimer = Observable['timer'];
    let timerSpy = jasmine.createSpy('timer');

    let sessionTimeout = 1800000; // This needs to match SessionLengthMilliseconds in UserService
    beforeEach(() => {
      // This is a hack to override the static Observable.timer method with our own spy,
      // so we can share the subscription it creates, and validate the action.
      // TODO There *has* to be a better way to do this - without a major hack like this!  Maybe I designed the implementation wrong.
      Observable['timer'] = timerSpy;
    });

    afterEach(() => {
      // Reset the hack so Observable.timer goes back to normal
      Observable['timer'] = originalObservableTimer;
    });

    it('should set an unauthenticated user and cancel the existing subscription', () => {
      let user = new User();
      user.token = undefined;

      let subscription = jasmine.createSpyObj<Subscription>('subscription', ['unsubscribe']);
      fixture['refreshTimeout'] = subscription;

      fixture.setUser(user);
      expect(cookieService.putObject).toHaveBeenCalledWith('user', user, jasmine.any(CookieOptions));
      expect(subscription.unsubscribe).toHaveBeenCalled();
      expect(fixture['refreshTimeout']).not.toBeDefined();

      // Make sure the cookie expiration was set appropriately.
      // It should be within 100ms or so of the current time + the timeout defined above
      let expiration = (<jasmine.Spy>cookieService.putObject).calls.mostRecent().args[2].expires.getTime();
      let expectedDate = moment().add(sessionTimeout, 'milliseconds').toDate().getTime();
      expect((expectedDate - expiration) < 100).toBeTruthy();
    });

    it('should set an authenticated user and create a new subscription', () => {
      let user = new User();
      user.token = 'tok123';

      let shared = Observable.of({}).share();
      timerSpy.and.returnValue(shared);

      fixture['refreshTimeout'] = undefined;

      fixture.setUser(user);
      expect(cookieService.putObject).toHaveBeenCalledWith('user', user, jasmine.any(CookieOptions));
      expect(fixture['refreshTimeout']).toBeDefined();

      shared.subscribe(() => {
        expect(loginRedirectService.redirectToLogin).toHaveBeenCalledWith('/current/page');
      });
    });
  });

  describe('#isLoggedIn', () => {
    let user: User;
    beforeEach(() => {
      user = jasmine.createSpyObj<User>('user', ['isLoggedIn']);
      spyOn(fixture, 'getUser').and.returnValue(user);
    });

    it('should return true if the user is logged in', () => {
      (<jasmine.Spy>user.isLoggedIn).and.returnValue(true);
      expect(fixture.isLoggedIn()).toBeTruthy();
    });

    it('should return false if the user is not logged in', () => {
      (<jasmine.Spy>user.isLoggedIn).and.returnValue(false);
      expect(fixture.isLoggedIn()).toBeFalsy();
    });
  });

  describe('#logOut', () => {
    it('should log out and update cookie', () => {
      let user = jasmine.createSpyObj<User>('user', ['logOut']);
      spyOn(fixture, 'getUser').and.returnValue(user);
      spyOn(fixture, 'setUser').and.stub();
      fixture.logOut();
      expect(fixture.getUser).toHaveBeenCalledTimes(1);
      expect(fixture.setUser).toHaveBeenCalledWith(user);
      expect(user.logOut).toHaveBeenCalled();
    });
  });
});
