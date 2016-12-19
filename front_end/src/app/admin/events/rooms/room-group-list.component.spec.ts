import { RoomGroupListComponent } from './room-group-list.component';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Location } from '@angular/common';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';
import { Room } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { Observable } from 'rxjs';

fdescribe('RoomGroupListComponent', () => {
  let fixture: RoomGroupListComponent;
  let apiService: ApiService;
  let headerService: HeaderService;
  let rootService: RootService;
  let adminService: AdminService;
  let location: Location;
  const serviceEventId = 2893749283;

  beforeEach(() => {
    let route: ActivatedRoute;
    route = new ActivatedRoute();
    route.snapshot = new ActivatedRouteSnapshot();
    route.snapshot.params = { eventId: serviceEventId };
    route.snapshot.queryParams = { tab: '' }
    apiService = jasmine.createSpyObj<ApiService>('apiService', ['getEventMaps', 'getEvent']);
    adminService = jasmine.createSpyObj<ApiService>('adminService', ['getRoomGroups']);
    headerService = jasmine.createSpyObj<ApiService>('headerService', ['announceEvent']);

    fixture = new RoomGroupListComponent(apiService, adminService, rootService, route, headerService, location);
  });

  describe('#ngOnInit', () => {
    it('should get event maps and set active event (adventure club or service event)', () => {
      const serviceEvent = {
        'EventTitle': 'Cool Event 83',
        'EventId': serviceEventId,
        'EventTypeId': 20,
      };
      const adventureClubSubevent = {
        'EventTitle': 'Cool Event 84',
        'EventId': '92398420398',
        'ParentEventId': serviceEvent.EventId,
      };
      const room = {
        'RoomId': '92398420398',
        'AdventureClub': true
      };
      (<jasmine.Spy>apiService.getEventMaps).and.returnValue(Observable.of([serviceEvent, adventureClubSubevent]));
      (<jasmine.Spy>apiService.getEvent).and.returnValue(Observable.of(serviceEvent));
      (<jasmine.Spy>adminService.getRoomGroups).and.returnValue(Observable.of(Room.fromJson(room)));

      fixture.ngOnInit();
      expect(apiService.getEventMaps).toHaveBeenCalledWith(serviceEvent.EventId);
      expect(fixture.eventToUpdate).toEqual(serviceEvent);
    });
  });
});
