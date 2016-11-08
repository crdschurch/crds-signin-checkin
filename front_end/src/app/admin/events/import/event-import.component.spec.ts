import { EventImportComponent } from './event-import.component';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { AdminService } from '../../admin.service';
import { Event } from '../../events/event';
import { HeaderService } from '../../header/header.service';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('RoomListComponent', () => {
  let fixture: EventImportComponent;
  let eventId: 54321;
  let eventSiteId: 90210;

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
    fixture = new EventImportComponent(route, adminService, headerService);
    fixture.targetEvent = null;
    fixture.events = null;
    fixture.sourceEventDate = null;
  });

  describe('#ngOnInit', () => {
    it('should get target event and list of source events', () => {
      let targetEventStartDate = moment().add(1, 'days');
      let sourceEventDate = moment(targetEventStartDate).startOf('day').subtract(7, 'days').toDate();

      let targetEvent = new Event();
      targetEvent.EventId = eventId;
      targetEvent.EventSiteId = eventSiteId;
      targetEvent.EventStartDate = targetEventStartDate.toISOString();
      (<jasmine.Spy>(adminService.getEvent)).and.returnValue(Observable.of(targetEvent));

      spyOn(fixture, 'changeSourceEventDate').and.callFake(() => { });

      fixture.ngOnInit();

      expect(adminService.getEvent).toHaveBeenCalledWith(eventId);
      expect(fixture.changeSourceEventDate).toHaveBeenCalled();
      expect(headerService.announceEvent).toHaveBeenCalledWith(targetEvent);

      expect(fixture.targetEvent).toBe(targetEvent);
      expect(fixture.sourceEventDate).toEqual(sourceEventDate);
    });
  });

  describe('#changeSourceEventDate', () => {
    it('should get events and sort by date', () => {
      let sourceEventDate = moment().subtract(1, 'days');
      fixture.sourceEventDate = sourceEventDate.toDate();

      let sourceEvents: Event[] = [ new Event(), new Event() ];
      sourceEvents[0].EventId = 123;
      sourceEvents[0].EventStartDate = moment(sourceEventDate).add(7, 'days').toISOString();
      sourceEvents[1].EventId = 456;
      sourceEvents[1].EventStartDate = moment(sourceEventDate).subtract(7, 'days').toISOString();
      (<jasmine.Spy>(adminService.getEvents)).and.returnValue(Observable.of(sourceEvents));

      let targetEvent = new Event();
      targetEvent.EventId = eventId;
      targetEvent.EventSiteId = eventSiteId;
      fixture.targetEvent = targetEvent;

      fixture.changeSourceEventDate(null);

      expect(adminService.getEvents).toHaveBeenCalledWith(sourceEventDate.toDate(), sourceEventDate.toDate(), eventSiteId);
      expect(fixture.events).toBeDefined();
      expect(fixture.events.length).toEqual(2);
      expect(fixture.events[0].EventId).toEqual(456);
      expect(fixture.events[1].EventId).toEqual(123);
    });
  });

});
