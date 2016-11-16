import { Observable } from 'rxjs';
import { ChildSigninComponent } from './child-signin.component';
import { ActivatedRoute, UrlSegment } from '@angular/router';

let fixture: ChildSigninComponent;
let route: ActivatedRoute;
route = new ActivatedRoute();

fdescribe('ChildSigninComponent', () => {
  describe('#isStepActive', () => {

    describe('while on step 1', () => {
      beforeEach(() => {
        route.url = Observable.of(new UrlSegment(paths: ['/child-signin/search']));
        fixture = new ChildSigninComponent(route);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeFalsy();
        expect(fixture.isStepActive(3)).toBeFalsy();
      });
    });

    describe('while on step 2', () => {
      beforeEach(() => {
        route.url = '/child-signin/available-children/5138887777';
        fixture = new ChildSigninComponent(route);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeTruthy();
        expect(fixture.isStepActive(3)).toBeFalsy();
      });
    });

    describe('while on step 3', () => {
      beforeEach(() => {
        route.url = '/child-signin/assignment';
        fixture = new ChildSigninComponent(route);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeTruthy();
        expect(fixture.isStepActive(3)).toBeTruthy();
      });
    });
  });
});
