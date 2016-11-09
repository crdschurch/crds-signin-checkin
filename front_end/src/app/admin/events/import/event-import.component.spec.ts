import { EventImportComponent } from './event-import.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';
import { Event, Room } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('RoomListComponent', () => {
  let fixture: EventImportComponent;
  let eventId: 54321;
  let eventSiteId: 90210;

  let route: ActivatedRoute;
  let apiService: ApiService;
  let headerService: HeaderService;
  let rootService: RootService;
  let adminService: AdminService;
  let router: Router;

  beforeEach(() => {
    // Can't jasmine spy on properties, so have to create a stub for route.snapshot.params
    route = new ActivatedRoute();
    route.snapshot = new ActivatedRouteSnapshot();
    route.snapshot.params = {
      eventId: eventId
    };

    apiService = jasmine.createSpyObj<ApiService>('apiService', ['getEvent', 'getEvents']);
    headerService = jasmine.createSpyObj<HeaderService>('headerService', ['announceEvent']);
    rootService = jasmine.createSpyObj<RootService>('rootService', ['announceEvent']);
    adminService = jasmine.createSpyObj<AdminService>('adminService', ['importEvent']);
    router = jasmine.createSpyObj<Router>('router', ['navigate']);

    fixture = new EventImportComponent(route, apiService, headerService, rootService, adminService, router);
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
      (<jasmine.Spy>apiService.getEvent).and.returnValue(Observable.of(targetEvent));

      spyOn(fixture, 'changeSourceEventDate').and.callFake(() => { });

      fixture.ngOnInit();

      expect(apiService.getEvent).toHaveBeenCalledWith(eventId);
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

      let sourceEvents: Event[] = [ new Event(), new Event(), new Event() ];
      sourceEvents[0].EventId = 123;
      sourceEvents[0].EventStartDate = moment(sourceEventDate).add(7, 'days').toISOString();
      sourceEvents[1].EventId = eventId;
      sourceEvents[1].EventStartDate = moment(sourceEventDate).add(8, 'days').toISOString();
      sourceEvents[2].EventId = 456;
      sourceEvents[2].EventStartDate = moment(sourceEventDate).subtract(7, 'days').toISOString();
      (<jasmine.Spy>apiService.getEvents).and.returnValue(Observable.of(sourceEvents));

      let targetEvent = new Event();
      targetEvent.EventId = eventId; 
      targetEvent.EventSiteId = eventSiteId;
      fixture.targetEvent = targetEvent;

      fixture.changeSourceEventDate(null);

      expect(apiService.getEvents).toHaveBeenCalledWith(sourceEventDate.toDate(), sourceEventDate.toDate(), eventSiteId);
      expect(fixture.events).toBeDefined();
      expect(fixture.events.length).toEqual(2);
      expect(fixture.events[0].EventId).toEqual(456);
      expect(fixture.events[1].EventId).toEqual(123);
    });
  });

  describe('#backToEventRooms', () => {
    it('should navigate to the event rooms page', () => {
      (<jasmine.Spy>router.navigate).and.callFake(() => {});
      fixture.targetEvent = new Event();
      fixture.targetEvent.EventId = 123456;

      fixture.backToEventRooms();
      expect(router.navigate).toHaveBeenCalledWith([`/admin/events/${fixture.targetEvent.EventId}/rooms`]);
    });
  });

  describe('#submitForm', () => {
    let form: NgForm;
    let sourceEventId = 12345;
    let targetEventId = 67890;
    let targetEvent: Event;

    beforeEach(() => {
      form = <NgForm>{
        invalid: false
      };

      targetEvent = new Event();
      targetEvent.EventId = targetEventId;
    });

    it('should not import if the form is invalid', () => {
      fixture.sourceEventId = sourceEventId;
      form.invalid = true;
      expect(fixture.submitForm(form)).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckImportNoSourceEventSelected');
      expect(adminService.importEvent).not.toHaveBeenCalled();
    });

    it('should not import if the form is valid but no sourceEventId is set', () => {
      delete fixture.sourceEventId;
      form.invalid = false;
      expect(fixture.submitForm(form)).toBeFalsy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckImportNoSourceEventSelected');
      expect(adminService.importEvent).not.toHaveBeenCalled();
    });

    it('should go back to room list if successful', () => {
      fixture.sourceEventId = sourceEventId;
      fixture.targetEvent = targetEvent;
      form.invalid = false;

      (<jasmine.Spy>adminService.importEvent).and.callFake(() => {
        return Observable.of([new Room()]);
      });

      spyOn(fixture, 'backToEventRooms').and.callFake(() => {});

      expect(fixture.submitForm(form)).toBeTruthy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckEventImportSuccess');
      expect(fixture.backToEventRooms).toHaveBeenCalled();
    });

    it('should stay on import page if failure', () => {
      fixture.sourceEventId = sourceEventId;
      fixture.targetEvent = targetEvent;
      form.invalid = false;

      (<jasmine.Spy>adminService.importEvent).and.callFake(() => {
        return Observable.throw({});
      });

      spyOn(fixture, 'backToEventRooms').and.callFake(() => {});

      expect(fixture.submitForm(form)).toBeTruthy();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckEventImportFailure');
      expect(fixture.backToEventRooms).not.toHaveBeenCalled();
    });
  });

});
