/* tslint:disable:max-line-length */

import { ChildCheckinComponent } from './child-checkin.component';
import { MachineConfiguration } from '../setup/machine-configuration';
import { Observable } from 'rxjs/Observable';

let fixture: ChildCheckinComponent;
let thisMachineConfig: MachineConfiguration;
const event = { EventId: '123', IsCurrentEvent: false };
const event2 = { EventId: '456', IsCurrentEvent: false };
const eventCurrent = { EventId: '789', IsCurrentEvent: true };
let setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);
let adminService = jasmine.createSpyObj('adminService', ['getEvents']);

describe('Child Checkin Component', () => {
  describe('#ngOnInit', () => {
    describe('initalization', () => {
      beforeEach(() => {
        adminService.getEvents.and.returnValue(Observable.of([{}]));
        fixture = new ChildCheckinComponent(setupService, adminService);
      });

      it('should set kiosk config details from cookie and get today\'s events', () => {
        fixture.ngOnInit();
        expect(fixture.getKioskDetails()).toBe(thisMachineConfig);
        expect(setupService.getMachineDetailsConfigCookie).toHaveBeenCalled();
        expect(adminService.getEvents).toHaveBeenCalled();
      });
    });
    describe('setting the current event', () => {
      describe('and there is a current event', () => {
        beforeEach(() => {
          adminService.getEvents.and.returnValue(Observable.of([event, eventCurrent]));
          fixture = new ChildCheckinComponent(setupService, adminService);
        });

        it('should set where IsCurrentEvent is true', () => {
          fixture.ngOnInit();
          expect(fixture.selectedEvent.EventId).toEqual(eventCurrent.EventId);
        });
      });

      describe('and there is no current event', () => {
        beforeEach(() => {
          adminService.getEvents.and.returnValue(Observable.of([event, event2]));
          fixture = new ChildCheckinComponent(setupService, adminService);
        });

        it('should set first when no IsCurrentEvent', () => {
          fixture.ngOnInit();
          expect(fixture.selectedEvent.EventId).toEqual(event.EventId);
        });
      });

    });
  });
});
