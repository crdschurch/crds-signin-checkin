import { ChildCheckinComponent } from './child-checkin.component';
import { MachineConfiguration, Child, Event } from '../shared/models';
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
let childCheckinService = jasmine.createSpyObj('ChildCheckinService', ['selectEvent', 'getChildByCallNumber']);
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
       describe('override modal', () => {
         let fixture3;
         let fakeModal = { show: {}, hide: {} };
         beforeEach(() => {
           childCheckinService = jasmine.createSpyObj('ChildCheckinService',
              ['getChildByCallNumber', 'forceChildReload', 'overrideChildIntoRoom']);
           childCheckinService.getChildByCallNumber.and.returnValue(Observable.of());
           childCheckinService.forceChildReload.and.returnValue(Observable.of());
           childCheckinService.overrideChildIntoRoom.and.returnValue(Observable.of(new Child()));
           fixture3 = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
           fixture3.callNumber = '431';
           fixture3.overrideChild = new Child();
           fixture3.overrideChild.EventParticipantId = 444;
           fixture3.selectedEvent = new Event();
           fixture3.selectedEvent.EventId = 333;
           fixture3.kioskDetails = { RoomId: 444 };
           spyOn(fakeModal, 'show').and.callFake(() => {});
           spyOn(fakeModal, 'hide').and.callFake(() => {});
           fixture3.childSearchModal = fakeModal;
         });
         describe('#delete', () => {
           it('should delete a digit from call number', () => {
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
         describe('#setCallNumber', () => {
           it('should set the call number and call override checkin if four digits', () => {
             fixture3.setCallNumber('8');
             expect(fixture3.callNumber).toEqual('4318');
             expect(childCheckinService.getChildByCallNumber).toHaveBeenCalledWith(333, '4318', 444);
          });
         });
         describe('#overrideCheckin', () => {
           describe('success', () => {
             it('should hide modal, show message, call to have children reload', () => {
               fixture3.overrideCheckin();
               expect(fakeModal.hide).toHaveBeenCalled();
               expect(rootService.announceEvent).toHaveBeenCalledWith('checkinOverrideSuccess');
               expect(childCheckinService.forceChildReload).toHaveBeenCalled();
             });
           });
           describe('error (room over capacity)', () => {
             beforeEach(() => {
                childCheckinService.overrideChildIntoRoom.and.returnValue(Observable.throw('capacity'));
                fixture3 = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
             });
             it('should show specific error message', () => {
               fixture3.overrideCheckin();
               expect(rootService.announceEvent).toHaveBeenCalledWith('checkinOverrideRoomCapacityError');
             });
           });
           describe('error (room closed)', () => {
             beforeEach(() => {
                childCheckinService.overrideChildIntoRoom.and.returnValue(Observable.throw('closed'));
                fixture3 = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
             });
             it('should show specific error message', () => {
               fixture3.overrideCheckin();
               expect(rootService.announceEvent).toHaveBeenCalledWith('checkinOverrideRoomClosedError');
             });
           });
           describe('error (general)', () => {
             beforeEach(() => {
                childCheckinService.overrideChildIntoRoom.and.returnValue(Observable.throw('error!'));
                fixture3 = new ChildCheckinComponent(setupService, apiService, childCheckinService, rootService);
             });
             it('should show specific error message', () => {
               fixture3.overrideCheckin();
               expect(rootService.announceEvent).toHaveBeenCalledWith('generalError');
             });
           });
         });
       });
    });
  });
});
