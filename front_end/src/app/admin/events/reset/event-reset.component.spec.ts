import { EventResetComponent } from './event-reset.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { AdminService } from '../../admin.service';
import { Event } from '../../events/event';
import { HeaderService } from '../../header/header.service';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('RoomListComponent', () => {
  let fixture: EventResetComponent;
  let eventId: 54321;

  let route: ActivatedRoute;
  let adminService: AdminService;
  let headerService: HeaderService;

  beforeEach(() => {
    // Can't jasmine spy on properties, so have to create a stub for route.snapshot.params
    route = new ActivatedRoute();
    route.snapshot = new ActivatedRouteSnapshot();
    route.snapshot.params = {
      eventId: eventId
    };

    adminService = <AdminService>jasmine.createSpyObj('adminService', ['getEvent', 'getEvents']);
    headerService = <HeaderService>jasmine.createSpyObj('headerService', ['announceEvent']);
    fixture = new EventResetComponent(route, adminService, headerService);
    fixture.targetEvent = null;
  });

  describe('#ngOnInit', () => {
    it('should get target event', () => {
      let targetEventStartDate = moment().add(1, 'days');

      let targetEvent = new Event();
      targetEvent.EventId = eventId;
      targetEvent.EventStartDate = targetEventStartDate.toISOString();
      (<jasmine.Spy>(adminService.getEvent)).and.returnValue(Observable.of(targetEvent));

      fixture.ngOnInit();

      expect(adminService.getEvent).toHaveBeenCalledWith(eventId);
      expect(headerService.announceEvent).toHaveBeenCalledWith(targetEvent);

      expect(fixture.targetEvent).toBe(targetEvent);
    });
  });
});
