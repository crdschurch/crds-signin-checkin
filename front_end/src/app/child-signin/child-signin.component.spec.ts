// import { Observable } from 'rxjs';
import { ChildSigninComponent } from './child-signin.component';
import { Router } from '@angular/router';

let fixture: ChildSigninComponent;
let router: Router;


fdescribe('ChildSigninComponent', () => {
  describe('#isStepActive', () => {

    describe('while on step 1', () => {
      beforeEach(() => {
        router = <Router> {
          routerState: {
            snapshot: {
              url: '/child-signin/search'
            }
          },
          url: '/child-signin/search'
        };
        fixture = new ChildSigninComponent(router);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeFalsy();
        expect(fixture.isStepActive(3)).toBeFalsy();
      });
    });

    describe('while on step 2', () => {
      beforeEach(() => {
        router = <Router> {
          routerState: {
            snapshot: {
              url: '/child-signin/available-children/5138887777'
            }
          },
          url: '/child-signin/available-children/5138887777'
        };
        fixture = new ChildSigninComponent(router);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeTruthy();
        expect(fixture.isStepActive(3)).toBeFalsy();
      });
    });

    describe('while on step 3', () => {
      beforeEach(() => {
        router = <Router> {
          routerState: {
            snapshot: {
              url: '/child-signin/assignment'
            }
          },
          url: '/child-signin/assignment'
        };
        fixture = new ChildSigninComponent(router);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeTruthy();
        expect(fixture.isStepActive(3)).toBeTruthy();
      });
    });
  });
});
