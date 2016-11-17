import { LoginRedirectService } from './';
import { Router } from '@angular/router';

describe('LoginRedirectService', () => {
  let fixture: LoginRedirectService;
  let router: Router;

  beforeEach(() => {
    router = jasmine.createSpyObj<Router>('router', ['navigate']);
    fixture = new LoginRedirectService(router);
  });

  describe('#redirectToLogin', () => {
    it('should store a default target and navigate to login page', () => {
      fixture.redirectToLogin();
      expect(fixture['originalTarget']).toEqual('/admin/events');
      expect(router.navigate).toHaveBeenCalledWith(['/admin/sign-in']);
    });

    it('should store the specified target and navigate to login page', () => {
      fixture.redirectToLogin('/some/protected/page');
      expect(fixture['originalTarget']).toEqual('/some/protected/page');
      expect(router.navigate).toHaveBeenCalledWith(['/admin/sign-in']);
    });
  });

  describe('#redirectToTarget', () => {
    it('should navigate to original target if there is one', () => {
      fixture['originalTarget'] = '/some/protected/page';
      fixture.redirectToTarget('/dont/go/here');
      expect(router.navigate).toHaveBeenCalledWith(['/some/protected/page']);
    });

    it('should navigate to default if no original target or specified target', () => {
      fixture['originalTarget'] = undefined;
      fixture.redirectToTarget();
      expect(router.navigate).toHaveBeenCalledWith(['/admin/events']);
    });

    it('should navigate to specified target if no original target', () => {
      fixture['originalTarget'] = undefined;
      fixture.redirectToTarget('/go/here');
      expect(router.navigate).toHaveBeenCalledWith(['/go/here']);
    });

  });
});
