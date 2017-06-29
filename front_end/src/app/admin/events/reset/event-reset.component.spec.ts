import { EventResetComponent } from './event-reset.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { ApiService, RootService } from '../../../shared/services';
import { Event} from '../../../shared/models';
import { Observable } from 'rxjs';
import { AdminService } from '../../admin.service';

import * as moment from 'moment';

describe('RoomListComponent', () => {
  let fixture: EventResetComponent;
  let eventId: 54321;

  let route: ActivatedRoute;
  let router: Router;
  let apiService: ApiService;
  let adminService: AdminService;
  let rootService: RootService;

  beforeEach(() => {
    // Can't jasmine spy on properties, so have to create a stub for route.snapshot.params
    route = new ActivatedRoute();
    route.snapshot = new ActivatedRouteSnapshot();
    route.snapshot.params = {
      eventId: eventId
    };

    apiService = jasmine.createSpyObj<ApiService>('apiService', ['getEvent']);
    fixture = new EventResetComponent(route, router, rootService, apiService, adminService);
    fixture.targetEvent = null;
  });

  describe('#ngOnInit', () => {
    it('should get target event', () => {
      let targetEventStartDate = moment().add(1, 'days');

      let targetEvent = new Event();
      targetEvent.EventId = eventId;
      targetEvent.EventStartDate = targetEventStartDate.toISOString();
      (<jasmine.Spy>apiService.getEvent).and.returnValue(Observable.of(targetEvent));

      fixture.ngOnInit();

      expect(apiService.getEvent).toHaveBeenCalledWith(eventId);
      expect(fixture.targetEvent).toBe(targetEvent);
    });
  });
});
