/* tslint:disable:max-line-length */

import { ChildCheckinComponent } from './child-checkin.component';
import { MachineConfiguration } from '../setup/machine-configuration';
import { Observable } from 'rxjs/Observable';

let fixture: ChildCheckinComponent;
let setupService: any;
let adminService: any;
let thisMachineConfig: MachineConfiguration;
let event = { EventId: '123', IsCurrentEvent: false };
let event2 = { EventId: '456', IsCurrentEvent: false };
let eventCurrent = { EventId: '789', IsCurrentEvent: true };

fdescribe('Child Checkin Component', () => {
  describe('#ngOnInit', () => {
    describe('initalization', () => {
      beforeEach(() => {
        setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
        setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);
        adminService = jasmine.createSpyObj('adminService', ['getEvents']);
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
      describe('should set where IsCurrentEvent is true', () => {
        beforeEach(() => {
          setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
          setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);
          adminService = jasmine.createSpyObj('adminService', ['getEvents']);
          adminService.getEvents.and.returnValue(Observable.of([event, eventCurrent]));
          fixture = new ChildCheckinComponent(setupService, adminService);
        });

        it('should set kiosk config details from cookie and get today\'s events', () => {
          fixture.ngOnInit();
          expect(fixture.selectedEvent.EventId).toEqual(eventCurrent.EventId);
        });
      });

      describe('should set first when no IsCurrentEvent', () => {
        beforeEach(() => {
          setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
          setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);
          adminService = jasmine.createSpyObj('adminService', ['getEvents']);
          adminService.getEvents.and.returnValue(Observable.of([event, event2]));
          fixture = new ChildCheckinComponent(setupService, adminService);
        });

        it('should set kiosk config details from cookie and get today\'s events', () => {
          fixture.ngOnInit();
          expect(fixture.selectedEvent.EventId).toEqual(event.EventId);
        });
      });

    });
  });
});
