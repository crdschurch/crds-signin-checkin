import { SearchComponent } from './search.component';
import { ChildSigninService } from '../child-signin.service';
import { Observable } from 'rxjs/Observable';
import { RootService } from '../../shared/services';
import { Router } from '@angular/router';
import { EventParticipants, Child } from '../../shared/models';

let fixture: SearchComponent;
let routerStub: Router;
let rootServiceStub: RootService;
let childSigninService: ChildSigninService;

describe('SearchComponent', () => {

  beforeEach(() => {
    routerStub = jasmine.createSpyObj<Router>('router', ['navigate']);
    rootServiceStub = jasmine.createSpyObj<RootService>('rootService', ['announceEvent']);
    childSigninService = jasmine.createSpyObj<ChildSigninService>('childSigninService', ['getChildrenByPhoneNumber', 'reset']);

    fixture = new SearchComponent(routerStub, childSigninService, rootServiceStub);
  });

  describe('#ngOnInit', () => {
    it('should reset the service so nothing is cached', () => {
      fixture.ngOnInit();
      expect(childSigninService.reset).toHaveBeenCalled();
    });
  });

  describe('#constructor', () => {
    it('should intialize to empty phone number', () => {
      expect(fixture.phoneNumber).toEqual('');
    });
  });

  describe('#setPhoneNumber', () => {
    it('should add a number to the current phone number', () => {
      fixture.setPhoneNumber('1');
      expect(fixture.phoneNumber).toEqual('1');

      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      expect(fixture.phoneNumber).toEqual('123');
    });
  });

  describe('#delete', () => {
    it('should not make null or undefined if deleting a blank string', () => {
      fixture.delete();
      expect(fixture.phoneNumber).toEqual('');
    });

    it('should delete the last character', () => {
      fixture.setPhoneNumber('1');
      expect(fixture.phoneNumber).toEqual('1');

      fixture.delete();
      expect(fixture.phoneNumber).toEqual('');

      fixture.setPhoneNumber('1');
      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      expect(fixture.phoneNumber).toEqual('123');

      fixture.delete();
      expect(fixture.phoneNumber).toEqual('12');
    });
  });

  describe('#clear', () => {
    it('should not make null or undefined if clearing a blank string', () => {
      fixture.clear();
      expect(fixture.phoneNumber).toEqual('');
    });

    it('should clear if any values', () => {
      fixture.setPhoneNumber('1');
      expect(fixture.phoneNumber).toEqual('1');

      fixture.clear();
      expect(fixture.phoneNumber).toEqual('');

      fixture.setPhoneNumber('1');
      fixture.setPhoneNumber('2');
      fixture.setPhoneNumber('3');
      expect(fixture.phoneNumber).toEqual('123');

      fixture.clear();
      expect(fixture.phoneNumber).toEqual('');
    });
  });

  describe('#next', () => {
    it('should emit error if phone number is not valid', () => {
      fixture.phoneNumber = '123';
      fixture.next();
      expect(routerStub.navigate).not.toHaveBeenCalled();
      expect(rootServiceStub.announceEvent).toHaveBeenCalledWith('kcChildSigninPhoneNumberNotValid');
    });

    describe('with valid phone number', () => {
      let phoneNumber = '5135551212';
      beforeEach(() => {
        childSigninService = jasmine.createSpyObj<ChildSigninService>('childSigninService', ['getChildrenByPhoneNumber']);
        fixture = new SearchComponent(routerStub, childSigninService, rootServiceStub);

        fixture.phoneNumber = phoneNumber;
      });

      it('should emit an error if there is a problem searching', () => {
        (<jasmine.Spy>childSigninService.getChildrenByPhoneNumber).and.returnValue(Observable.throw('Error searching children'));
        fixture.next();
        expect(childSigninService.getChildrenByPhoneNumber).toHaveBeenCalledWith(phoneNumber);
        expect(routerStub.navigate).not.toHaveBeenCalled();
        expect(rootServiceStub.announceEvent).toHaveBeenCalledWith('generalError');
      });

      it('should emit a warning if no children are found', () => {
        (<jasmine.Spy>childSigninService.getChildrenByPhoneNumber).and.returnValue(Observable.of(new EventParticipants()));
        fixture.next();
        expect(childSigninService.getChildrenByPhoneNumber).toHaveBeenCalledWith(phoneNumber);
        expect(routerStub.navigate).not.toHaveBeenCalled();
        expect(rootServiceStub.announceEvent).toHaveBeenCalledWith('kcChildSigninNoAvailableChildren');
      });

      it('should navigate if children are found', () => {
        let eventParticipants = new EventParticipants();
        eventParticipants.Participants = [
          new Child(),
          new Child()
        ];
        (<jasmine.Spy>childSigninService.getChildrenByPhoneNumber).and.returnValue(Observable.of(eventParticipants));
        fixture.next();
        expect(childSigninService.getChildrenByPhoneNumber).toHaveBeenCalledWith(phoneNumber);
        expect(routerStub.navigate).toHaveBeenCalledWith(['/child-signin/available-children', phoneNumber]);
      });
    });
  });
});
