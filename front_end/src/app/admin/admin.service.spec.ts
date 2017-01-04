/* tslint:disable:max-line-length */

import { AdminService } from './admin.service';
import { HttpClientService } from '../shared/services';
import { Response } from '@angular/http';
import { Room, NewFamily } from '../shared/models';
import { Observable } from 'rxjs';

describe('AdminService', () => {
  let fixture: AdminService;
  let httpClientService: HttpClientService;
  let responseObject: Response;
  let response: Observable<Response>;

  beforeEach(() => {
    httpClientService = jasmine.createSpyObj<HttpClientService>('httpClientService', ['get', 'post', 'put']);
    responseObject = jasmine.createSpyObj('response', ['json']);
    response = Observable.of(responseObject);

    fixture = new AdminService(httpClientService);
  });

  it('should successfully get list of Rooms', () => {
    let eventId = '4525323';
    let expectedRooms = [ new Room() ];
    (<jasmine.Spy>httpClientService.get).and.returnValue(response);
    (<jasmine.Spy>responseObject.json).and.returnValue(expectedRooms);

    fixture.getRooms(eventId).subscribe((rooms: Room[]) => {
      expect(rooms).toBe(expectedRooms);
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms`);
    });
  });

  it('should successfully update a Room', () => {
    let eventId = '4525323';
    let roomId = '185';
    let eventRoomId = '5150';
    let room = new Room();
    room.EventId = eventId;
    room.EventRoomId = eventRoomId;
    room.RoomId = roomId;

    let expectedRoom = new Room();
    (<jasmine.Spy>httpClientService.put).and.returnValue(response);
    (<jasmine.Spy>responseObject.json).and.returnValue(expectedRoom);

    fixture.updateRoom(eventId, roomId, room).subscribe((updatedRoom: Room) => {
      expect(updatedRoom).toBe(expectedRoom);
      expect(httpClientService.put).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`, room);
    });
  });

  it('should successfully get list of RoomGroups', () => {
    let eventId = '4525323';
    let roomId = '185';

    let expectedRoom = Room.fromJson({EventRoomId: '12345'});

    (<jasmine.Spy>httpClientService.get).and.returnValue(response);
    (<jasmine.Spy>responseObject.json).and.returnValue(expectedRoom);

    fixture.getRoomGroups(eventId, roomId).subscribe((roomWithGroups: Room) => {
      expect(roomWithGroups).toEqual(expectedRoom);
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}/groups`);
    });
  });

  describe('#importEvent', () => {
    it('should return list of rooms imported', () => {
      let rooms: Room[] = [
        new Room(),
        new Room()
      ];
      rooms[0].EventRoomId = '12345';
      rooms[1].EventRoomId = '67890';

      let destinationEventId = 123;
      let sourceEventId = 456;

      (<jasmine.Spy>httpClientService.put).and.returnValue(response);
      (<jasmine.Spy>responseObject.json).and.returnValue(rooms);

      fixture.importEvent(destinationEventId, sourceEventId).subscribe((r) => {
        expect(httpClientService.put).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/events/${destinationEventId}/import/${sourceEventId}`, null, null);
      });

    });
  });

  describe('#createNewFamily', () => {
    it('should return list of rooms imported', () => {
      let family = new NewFamily();

      (<jasmine.Spy>httpClientService.post).and.returnValue(response);
      (<jasmine.Spy>responseObject.json).and.returnValue(undefined);

      fixture.createNewFamily(family).subscribe((res) => {
        expect(httpClientService.post).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/signin/newfamily`, family);
      });

    });
  });

});
