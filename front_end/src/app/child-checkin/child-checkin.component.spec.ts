/* tslint:disable:max-line-length */

import { ChildCheckinComponent } from './child-checkin.component';
import { MachineConfiguration } from '../setup/machine-configuration';

let fixture: ChildCheckinComponent;
let setupService: any;
let thisMachineConfig: MachineConfiguration;

describe('Child Checkin Component', () => {
  beforeEach(() => {
    setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
    setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);

    fixture = new ChildCheckinComponent(setupService);
  });

  describe('#ngOnInit', () => {
    it('should set kiosk config details from cookie', () => {
      fixture.ngOnInit();
      expect(fixture.getKioskDetails()).toBe(thisMachineConfig);
      expect(setupService.getMachineDetailsConfigCookie).toHaveBeenCalled();
    });
  });
});
