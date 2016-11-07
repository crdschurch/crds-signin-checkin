/* tslint:disable:max-line-length */

import { ChildCheckinComponent } from './child-checkin.component';
import { MachineConfiguration } from '../setup/machine-configuration';
import { Observable } from 'rxjs/Observable';

let fixture: ChildCheckinComponent;
let setupService: any;
let adminService: any;
let thisMachineConfig: MachineConfiguration;

describe('Child Checkin Component', () => {
  beforeEach(() => {
    setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
    setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);

    adminService = jasmine.createSpyObj('adminService', ['getEvents']);
    adminService.getEvents.and.returnValue(Observable.of([{}]));

    fixture = new ChildCheckinComponent(setupService, adminService);
  });

  describe('#ngOnInit', () => {
    it('should set kiosk config details from cookie and get today\'s events', () => {
      fixture.ngOnInit();
      expect(fixture.getKioskDetails()).toBe(thisMachineConfig);
      expect(setupService.getMachineDetailsConfigCookie).toHaveBeenCalled();
      expect(adminService.getEvents).toHaveBeenCalled();
    });
  });
});
