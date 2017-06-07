import { ChildSigninComponent } from './child-signin.component';
import { Router } from '@angular/router';
import { ChannelService } from '../shared/services';

let fixture: ChildSigninComponent;
function getRouterStub(url: string) {
  return <Router> {
    routerState: {
      snapshot: {
        url: url
      }
    },
    url: url
  };
};

let  channelService = <ChannelService>jasmine.createSpyObj('channelService', ['stop']);

describe('ChildSigninComponent', () => {
  describe('#isStepActive', () => {

    describe('while on step 1', () => {
      beforeEach(() => {
        fixture = new ChildSigninComponent(getRouterStub('/child-signin/search'), channelService);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeFalsy();
        expect(fixture.isStepActive(3)).toBeFalsy();
      });
    });

    describe('while on step 2', () => {
      beforeEach(() => {
        fixture = new ChildSigninComponent(getRouterStub('/child-signin/available-children/5138887777'), channelService);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeTruthy();
        expect(fixture.isStepActive(3)).toBeFalsy();
      });
    });

    describe('while on step 3', () => {
      beforeEach(() => {
        fixture = new ChildSigninComponent(getRouterStub('/child-signin/assignment'), channelService);
      });
      it('should activate the breadcrumb step', () => {
        expect(fixture.isStepActive(1)).toBeTruthy();
        expect(fixture.isStepActive(2)).toBeTruthy();
        expect(fixture.isStepActive(3)).toBeTruthy();
      });
    });
  });
});
