import { RoomListComponent } from './room-list.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AdminService } from '../../admin.service';
import { Event, Room } from '../../../shared/models';
import { ApiService } from '../../../shared/services';
import { RootService } from '../../../shared/services/root.service';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('RoomListComponent', () => {
  let fixture: RoomListComponent;
  const eventId = 54321;

  let route: ActivatedRoute;

  let apiService: ApiService;
  let adminService: AdminService;
  let router: Router;
  let rootService: RootService;

  beforeEach(() => {
    // Can't jasmine spy on properties, so have to create a stub for route.snapshot.params
    route = new ActivatedRoute();
    route.snapshot = new ActivatedRouteSnapshot();
    route.snapshot.params = {
      eventId: eventId
    };

    adminService = <AdminService>jasmine.createSpyObj('adminService', ['getRooms', 'getUnassignedGroups']);
    apiService = <ApiService>jasmine.createSpyObj('apiService', ['getEvent']);
    router = <Router>jasmine.createSpyObj('router', ['navigate']);
    rootService = <RootService>jasmine.createSpyObj('rootService', ['announceEvent']);
    fixture = new RoomListComponent(route, adminService, apiService, router, rootService);
    fixture.event = new Event();
  });

  describe('#ngOnInit', () => {
    it('should get rooms and event details', () => {
      let rooms: Room[] = [ new Room(), new Room() ];
      rooms[0].RoomId = '123';
      rooms[1].RoomId = '456';
      (<jasmine.Spy>(adminService.getRooms)).and.returnValue(Observable.of(rooms));
      (<jasmine.Spy>(adminService.getUnassignedGroups)).and.returnValue(Observable.of());

      let event = new Event();
      event.EventId = eventId;
      (<jasmine.Spy>(apiService.getEvent)).and.returnValue(Observable.of(event));
      spyOn(fixture, 'setRouteData').and.callFake(() => { });
      fixture.eventId = eventId.toString();
      // fixture.rooms = null;
      // fixture.event = null;

      fixture.ngOnInit();
      expect(adminService.getRooms).toHaveBeenCalledWith(fixture.eventId);
      expect(adminService.getUnassignedGroups).toHaveBeenCalled();
      expect(apiService.getEvent).toHaveBeenCalledWith(fixture.eventId);
      expect(fixture.setRouteData).toHaveBeenCalled();

      expect(fixture.rooms).toBe(rooms);
      expect(fixture.event).toBe(event);
    });
  });

  describe('#setRouteData', () => {
    it('should set eventId', () => {
      fixture.setRouteData();
      expect(parseInt(fixture.eventId, 10)).toBe(eventId);
    });
  });

  describe('#getOpenRooms and #getTotalRooms', () => {
    it('should calculate correct open and total rooms', () => {
      let rooms: Room[] = [ new Room(), new Room(), new Room(), new Room(), new Room() ];
      rooms[0].AllowSignIn = false;
      rooms[1].AllowSignIn = true;
      rooms[2].AllowSignIn = false;
      rooms[3].AllowSignIn = false;
      rooms[4].AllowSignIn = true;
      fixture.rooms = rooms;

      expect(fixture.getOpenRooms()).toEqual(2);
      expect(fixture.getTotalRooms()).toEqual(5);
    });
  });

  describe('#goToImport', () => {
    it('should not navigate if event is in the past', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().subtract(1, 'days').toISOString();
      fixture.event.EventId = eventId;
      fixture.setRouteData();
      fixture.goToImport();

      expect(router.navigate).not.toHaveBeenCalled();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckCannotOverwritePastEvent');
    });

    it('should navigate if event is in the future', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().add(1, 'days').toISOString();
      fixture.event.EventId = eventId;
      fixture.setRouteData();
      fixture.goToImport();

      expect(router.navigate).toHaveBeenCalledWith([`/admin/events/${eventId}/import/events`]);
      expect(rootService.announceEvent).not.toHaveBeenCalled();
    });
  });

  describe('#goToImportTemplate', () => {
    it('should not navigate if event is in the past', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().subtract(1, 'days').toISOString();
      fixture.event.EventId = eventId;
      fixture.setRouteData();
      fixture.goToImportTemplate();

      expect(router.navigate).not.toHaveBeenCalled();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckCannotOverwritePastEvent');
    });

    it('should navigate if event is in the future', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().add(1, 'days').toISOString();
      fixture.event.EventId = eventId;
      fixture.setRouteData();
      fixture.goToImportTemplate();

      expect(router.navigate).toHaveBeenCalledWith([`/admin/events/${eventId}/import/templates`]);
      expect(rootService.announceEvent).not.toHaveBeenCalled();
    });
  });

  describe('#goToReset', () => {
    it('should not navigate if event is in the past', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().subtract(1, 'days').toISOString();
      fixture.event.EventId = eventId;
      fixture.setRouteData();
      fixture.goToReset();

      expect(router.navigate).not.toHaveBeenCalled();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckCannotOverwritePastEvent');
    });

    it('should navigate if event is in the future', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().add(1, 'days').toISOString();
      fixture.event.EventId = eventId;
      fixture.setRouteData();
      fixture.goToReset();

      expect(router.navigate).toHaveBeenCalledWith([`/admin/events/${eventId}/reset`]);
      expect(rootService.announceEvent).not.toHaveBeenCalled();
    });
  });

  describe('should set hide rooms text', () => {
    it('should set label text for hidden rooms', () => {
      fixture.hideClosedRooms = true;
      fixture.toggleUnusedRooms();

      expect(fixture.hideClosedRooms).toBe(false);
    });

    it('should set label text for visible rooms', () => {
      fixture.hideClosedRooms = false;
      fixture.toggleUnusedRooms();

      expect(fixture.hideClosedRooms).toBe(true);
    });
  });

  describe('should add class if room open and has zero capacity', () => {
    it('should set label text for visible rooms', () => {
      let rooms: Room[] = [ new Room(), new Room()];
      rooms[0].Capacity = 2;
      rooms[0].AllowSignIn = true;
      rooms[1].Capacity = 0;
      rooms[1].AllowSignIn = true;
      fixture.rooms = rooms;

      expect(fixture.isZeroCapacityAndOpen(fixture.rooms[0])).toBe(false);
      expect(fixture.isZeroCapacityAndOpen(fixture.rooms[1])).toBe(true);
    });
  });

});
