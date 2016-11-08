/* tslint:disable:max-line-length */

import { TestBed } from '@angular/core/testing';
import { AdminService } from './admin.service';
import { HttpClientService } from '../shared/services/http-client.service';
import { Http, RequestOptions, Headers, Response } from '@angular/http';
import { MockBackend } from '@angular/http/testing';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { Room } from '../shared/models';

describe('AdminService', () => {
  let fixture: AdminService;
  let httpClientService: HttpClientService;
  let http: Http;
  let options: RequestOptions;
  let backend: MockBackend;
  let cookie: CookieService;
  let setupServiceStub: any;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [
        HttpClientService
      ],
    });
    backend = new MockBackend();
    options = new RequestOptions();
    options.headers = new Headers();
    http = new Http(backend, options);
    cookie = new CookieService(new CookieOptions());
    httpClientService = new HttpClientService(http, cookie);
    fixture = new AdminService(httpClientService, setupServiceStub);
  });

  it('should successfully get list of Events', () => {
    let responseObject = http.get('assets/mock-data/events-get.json');
    fixture.getEvents('10-11-16', '10-15-16', 1).subscribe((res: Response) => {
      expect(res.json()).toEqual(responseObject);
    });
  });

  it('should successfully get list of Rooms', () => {
    let responseObject = http.get('assets/mock-data/rooms-get.json');
    fixture.getRooms('4525323').subscribe((res: Response) => {
      expect(res.json()).toEqual(responseObject);
    });
  });

  it('should successfully update a Room', () => {
    let responseObject = http.get('assets/mock-data/rooms-update.json')
          .map(res => Room.fromJson(res.json()))
          .subscribe((room: Room) => {
            fixture.updateRoom('4525323', '185', room).subscribe((res: Response) => {
            expect(res.json()).toEqual(responseObject);
          });
    });
  });

  it('should successfully get list of RoomGroups', () => {
    let responseObject = http.get('assets/mock-data/room-groups-get.json');
    fixture.getRoomGroups('event-id-123', 'room-id-3242').subscribe((res: Response) => {
      expect(res.json()).toEqual(responseObject);
    });
  });

});
