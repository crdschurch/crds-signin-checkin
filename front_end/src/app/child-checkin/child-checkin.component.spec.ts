/* tslint:disable:max-line-length */

import { ChildCheckinComponent } from './child-checkin.component';
import { MachineConfiguration, Child } from '../shared/models';
import { RootService } from '../shared/services';
import { Observable } from 'rxjs/Observable';

let fixture: ChildCheckinComponent;
let thisMachineConfig: MachineConfiguration;
const event = { EventId: '123', EventStartDate: '2016-11-22 10:00:00', IsCurrentEvent: false };
const event2 = { EventId: '456', EventStartDate: '2016-11-22 09:00:00', IsCurrentEvent: false };
const eventCurrent = { EventId: '789', EventStartDate: '2016-11-22 08:00:00', IsCurrentEvent: true };
let setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);
let apiService = jasmine.createSpyObj('apiService', ['getEvents']);
let childCheckinService = jasmine.createSpyObj('ChildCheckinService', ['selectEvent']);
let rootService = jasmine.createSpyObj<RootService>('rootService', ['announceEvent']);

describe('ChildCheckinComponent', () => {
  describe('#ngOnInit', () => {
    describe('initalization', () => {
      beforeEach(() => {
        apiService.getEvents.and.returnValue(Observable.of([{}]));
        fixture = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
      });

      it('should set kiosk config details from cookie and get today\'s events', () => {
        fixture.ngOnInit();
        expect(fixture.getKioskDetails()).toBe(thisMachineConfig);
        expect(setupService.getMachineDetailsConfigCookie).toHaveBeenCalled();
        expect(apiService.getEvents).toHaveBeenCalled();
      });
    });
    describe('setting the current event', () => {
      describe('and there is a current event', () => {
        beforeEach(() => {
          apiService.getEvents.and.returnValue(Observable.of([event, eventCurrent]));
          fixture = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
        });

        it('should set where IsCurrentEvent is true', () => {
          fixture.ngOnInit();
          expect(fixture.selectedEvent.EventId).toEqual(eventCurrent.EventId);
        });
      });
      describe('and there is no current event', () => {
         let fixture2;
         beforeEach(() => {
           childCheckinService = jasmine.createSpyObj('ChildCheckinService', ['selectEvent']);
           apiService.getEvents.and.returnValue(Observable.of([event, event2]));
           fixture2 = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
         });

         it('should set first when no IsCurrentEvent', () => {
           fixture2.ngOnInit();
           expect(fixture2.selectedEvent.EventId).toEqual(event2.EventId);
         });
       });
       fdescribe('override modal', () => {
         let fixture3;
         beforeEach(() => {
           childCheckinService = jasmine.createSpyObj('ChildCheckinService', ['selectEvent']);
           apiService.getEvents.and.returnValue(Observable.of([event, event2]));
           fixture3 = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
           fixture3.callNumber = '432';
           fixture3.overrideChild = new Child();
           fixture3.overrideChild.EventParticipantId = 444;
         });
         describe('#delete', () => {
           it('should delete a digit from call number', () => {
             console.log("DEL", fixture3)
             fixture3.delete();
             expect(fixture3.callNumber).toEqual('43');
           });
         });
         describe('#clear', () => {
           it('should clear call number', () => {
             fixture3.clear();
             expect(fixture3.callNumber).toEqual('');
           });
         });
         describe('#resetShowChildModal', () => {
           it('should remove override child if defined', () => {
             expect(fixture3.overrideChild.EventParticipantId).toEqual(444);
             fixture3.resetShowChildModal();
             expect(fixture3.overrideChild.EventParticipantId).not.toBeDefined();
           });
         });
         describe('#setCallNumber', () => {  });
         describe('#overrideCheckin', () => {  });
       });
    });
  });
});
