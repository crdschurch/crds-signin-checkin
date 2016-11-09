/* tslint:disable:max-line-length */

import { TestBed } from '@angular/core/testing';
import { AdminService } from './admin.service';
import { HttpClientService } from '../shared/services/http-client.service';
import { Http, RequestOptions, Headers, Response } from '@angular/http';
import { MockBackend } from '@angular/http/testing';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { Room } from '../shared/models';
import { Observable } from 'rxjs';

describe('AdminService', () => {
  let fixture: AdminService;
  let httpClientService: HttpClientService;
  let http: Http;
  let options: RequestOptions;
  let backend: MockBackend;
  let cookie: CookieService;

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

    fixture = new AdminService(httpClientService);
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

  describe('#importEvent', () => {
    let httpClient: HttpClientService;
    let httpResponse: Response;
    beforeEach(() => {
      httpClient = jasmine.createSpyObj<HttpClientService>('httpClientService', ['put']);
      httpResponse = jasmine.createSpyObj<Response>('response', ['json']);

      fixture = new AdminService(httpClient);
    });

    it('should return list of rooms imported', () => {
      let rooms: Room[] = [
        new Room(),
        new Room()
      ];
      rooms[0].EventRoomId = '12345';
      rooms[1].EventRoomId = '67890';

      let destinationEventId = 123;
      let sourceEventId = 456;

      (<jasmine.Spy>httpClient.put).and.callFake(() => {
        return Observable.of(httpResponse);
      });

      (<jasmine.Spy>httpResponse.json).and.callFake(() => {
        return rooms;
      });

      let response = fixture.importEvent(destinationEventId, sourceEventId);
      response.subscribe((r) => {
        expect(httpClient.put).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/events/${destinationEventId}/import/${sourceEventId}`, null, null);
        expect(httpResponse.json).toHaveBeenCalled();
      });

    });
  });

});
