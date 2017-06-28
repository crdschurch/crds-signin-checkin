import { EventTemplatesListComponent } from './event-list.component';
import { AdminService } from '../../admin.service';
import { ApiService, RootService, SetupService } from '../../../shared/services';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('EventTemplatesListComponent', () => {
  let fixture: EventTemplatesListComponent;
  let apiService: ApiService;
  let rootService: RootService;
  let setupService: SetupService;

  beforeEach(() => {
    apiService = <ApiService>jasmine.createSpyObj('apiService', ['getEventTemplates']);
    rootService = <RootService>jasmine.createSpyObj('rootService', ['announceEvent']);
    setupService = <SetupService>jasmine.createSpyObj('setupService', ['getThisMachineConfiguration']);
    fixture = new EventTemplatesListComponent(apiService, rootService, setupService);
  });

  describe('#ngOnInit', () => {
    it('should call getEventTemplates', () => {
      (<jasmine.Spy>(apiService.getEventTemplates)).and.returnValue(Observable.of([]));
      fixture.ngOnInit();
      expect(apiService.getEventTemplates).toHaveBeenCalled();
      expect(fixture.isReady).toBeTruthy();
    });
  });

});
