/* tslint:disable:max-line-length */

import { ChildCheckinComponent } from './child-checkin.component';
import { MachineConfiguration } from '../setup/machine-configuration';
import { Observable } from 'rxjs/Observable';

let fixture: ChildCheckinComponent;
let setupService: any;
let thisMachineConfig: MachineConfiguration;

let adminServiceStub: any = {
  getEvents(startDate, endDate) {
    return Observable.of([{}]);
  }
};

describe('Child Checkin Component', () => {
  beforeEach(() => {
    setupService = jasmine.createSpyObj('setupService', ['getMachineDetailsConfigCookie']);
    setupService.getMachineDetailsConfigCookie.and.returnValue(thisMachineConfig);

    fixture = new ChildCheckinComponent(setupService, adminServiceStub);
  });

  describe('#ngOnInit', () => {
    it('should set kiosk config details from cookie', () => {
      fixture.ngOnInit();
      expect(fixture.getKioskDetails()).toBe(thisMachineConfig);
      expect(setupService.getMachineDetailsConfigCookie).toHaveBeenCalled();
    });
  });
});
