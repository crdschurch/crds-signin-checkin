import { RoomListComponent } from './room-list.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AdminService } from '../../admin.service';
import { Event, Room } from '../../../shared/models';
import { ApiService } from '../../../shared/services';
import { HeaderService } from '../../header/header.service';
import { RootService } from '../../../shared/services/root.service';
import { Observable } from 'rxjs';

import * as moment from 'moment';

describe('RoomListComponent', () => {
  let fixture: RoomListComponent;
  let eventId: 54321;

  let route: ActivatedRoute;

  let apiService: ApiService;
  let adminService: AdminService;
  let headerService: HeaderService;
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
    headerService = <HeaderService>jasmine.createSpyObj('headerService', ['announceEvent']);
    router = <Router>jasmine.createSpyObj('router', ['navigate']);
    rootService = <RootService>jasmine.createSpyObj('rootService', ['announceEvent']);
    fixture = new RoomListComponent(route, adminService, apiService, headerService, router, rootService);
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

      fixture.eventId = null;
      fixture.rooms = null;
      fixture.event = null;

      fixture.ngOnInit();
      expect(adminService.getRooms).toHaveBeenCalledWith(eventId);
      expect(adminService.getUnassignedGroups).toHaveBeenCalled();
      expect(apiService.getEvent).toHaveBeenCalledWith(eventId);
      expect(headerService.announceEvent).toHaveBeenCalledWith(event);

      expect(fixture.eventId).toEqual(eventId);
      expect(fixture.rooms).toBe(rooms);
      expect(fixture.event).toBe(event);
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
      fixture.goToImport();

      expect(router.navigate).not.toHaveBeenCalled();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckCannotOverwritePastEvent');
    });

    it('should navigate if event is in the future', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().add(1, 'days').toISOString();
      fixture.goToImport();

      expect(router.navigate).toHaveBeenCalledWith([`/admin/events/${eventId}/import`]);
      expect(rootService.announceEvent).not.toHaveBeenCalled();
    });
  });

  describe('#goToReset', () => {
    it('should not navigate if event is in the past', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().subtract(1, 'days').toISOString();
      fixture.goToReset();

      expect(router.navigate).not.toHaveBeenCalled();
      expect(rootService.announceEvent).toHaveBeenCalledWith('echeckCannotOverwritePastEvent');
    });

    it('should navigate if event is in the future', () => {
      fixture.event = new Event();
      fixture.event.EventStartDate = moment().add(1, 'days').toISOString();
      fixture.goToReset();

      expect(router.navigate).toHaveBeenCalledWith([`/admin/events/${eventId}/reset`]);
      expect(rootService.announceEvent).not.toHaveBeenCalled();
    });
  });

  describe('should set hide rooms text', () => {
    it('should set label text for hidden rooms', () => {
      fixture.hideClosedRooms = true;
      fixture.closedRoomsLabelText = 'Show All Rooms';
      fixture.toggleUnusedRooms();

      expect(fixture.hideClosedRooms).toBe(false);
      expect(fixture.closedRoomsLabelText).toBe('Hide Unused Rooms');
    });

    it('should set label text for visible rooms', () => {
      fixture.hideClosedRooms = false;
      fixture.closedRoomsLabelText = 'Hide Unused Rooms';
      fixture.toggleUnusedRooms();

      expect(fixture.hideClosedRooms).toBe(true);
      expect(fixture.closedRoomsLabelText).toBe('Show All Rooms');
    });
  });

});
