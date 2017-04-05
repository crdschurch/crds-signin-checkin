import { EventListComponent } from './event-list.component';
import { AdminService } from '../admin.service';
import { Event } from '../../shared/models';
import { ApiService, RootService, SetupService } from '../../shared/services';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('EventListComponent', () => {
  let fixture: EventListComponent;
  let apiService: ApiService;
  let rootService: RootService;
  let setupService: SetupService;

  beforeEach(() => {
    apiService = <ApiService>jasmine.createSpyObj('apiService', ['getSites', 'getEvents']);
    rootService = <RootService>jasmine.createSpyObj('rootService', ['announceEvent']);
    setupService = <SetupService>jasmine.createSpyObj('setupService', ['getThisMachineConfiguration']);
    fixture = new EventListComponent(apiService, rootService, setupService);
  });

  describe('#ngOnInit', () => {
    it('should call createWeekFilters, getThisMachineConfiguration, setupSite, getData', () => {
      (<jasmine.Spy>(setupService.getThisMachineConfiguration)).and.returnValue(Observable.of());

      fixture.ngOnInit();

      expect(fixture.weekFilters.length).toBe(7);
      expect(setupService.getThisMachineConfiguration).toHaveBeenCalled();
    });
  });

  describe('#getData', () => {
    it('should get sites then events', () => {
      (<jasmine.Spy>(apiService.getSites)).and.returnValue(Observable.of([]));

      fixture.getData();

      expect(apiService.getSites).toHaveBeenCalled();
    });
  });
});
