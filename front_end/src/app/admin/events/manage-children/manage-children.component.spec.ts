import { ManageChildrenComponent } from './manage-children.component';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Event, Child } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';
import { Observable } from 'rxjs';

describe('ManageChildrenComponent', () => {
  let fixture: ManageChildrenComponent;
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

    adminService = <AdminService>jasmine.createSpyObj('adminService', ['reverseSignin']);
    apiService = <ApiService>jasmine.createSpyObj('apiService', ['getEvent']);
    headerService = <HeaderService>jasmine.createSpyObj('headerService', ['announceEvent']);
    router = <Router>jasmine.createSpyObj('router', ['navigate']);
    rootService = <RootService>jasmine.createSpyObj('rootService', ['announceEvent']);
    fixture = new ManageChildrenComponent(route, apiService, headerService, rootService, adminService, router);
    fixture.children = new Array<Child>();
  });

  // describe('#ngOnInit', () => {
  //   // TODO: Implement...
  // (<jasmine.Spy>(adminService.getChildrenForEvent)).and.returnValue(Observable.of(children));
  // });

  describe('#reverseSignin', () => {
    it('should sign out a child', () => {
      let children: Child[] = [ new Child(), new Child() ];
      children[0].EventParticipantId = 123;
      children[1].EventParticipantId = 456;
      fixture.children = children;
      let eventParticipantId = children[1].EventParticipantId;

      (<jasmine.Spy>(adminService.reverseSignin)).and.returnValue(Observable.of());
      fixture.reverseSignin(children[1]);

      expect(adminService.reverseSignin).toHaveBeenCalledWith(eventParticipantId);
      expect(fixture.children[1] === undefined);
    });
  });
});
