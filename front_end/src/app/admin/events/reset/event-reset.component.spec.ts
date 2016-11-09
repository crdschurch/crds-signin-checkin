import { EventResetComponent } from './event-reset.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { ApiService } from '../../../shared/services';
import { Event} from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('RoomListComponent', () => {
  let fixture: EventResetComponent;
  let eventId: 54321;

  let route: ActivatedRoute;
  let apiService: ApiService;
  let headerService: HeaderService;

  beforeEach(() => {
    // Can't jasmine spy on properties, so have to create a stub for route.snapshot.params
    route = new ActivatedRoute();
    route.snapshot = new ActivatedRouteSnapshot();
    route.snapshot.params = {
      eventId: eventId
    };

    apiService = jasmine.createSpyObj<ApiService>('apiService', ['getEvent', 'getEvents']);
    headerService = jasmine.createSpyObj<HeaderService>('headerService', ['announceEvent']);
    fixture = new EventResetComponent(route, apiService, headerService);
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
      expect(headerService.announceEvent).toHaveBeenCalledWith(targetEvent);

      expect(fixture.targetEvent).toBe(targetEvent);
    });
  });
});
