/* tslint:disable:max-line-length */

import { AdminService } from './admin.service';
import { HttpClientService } from '../shared/services';
import { Response } from '@angular/http';
import { EventParticipants, Room, NewFamily, Child, Group, Contact } from '../shared/models';
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
    let expectedRooms = [ Room.fromJson({}) ];
    (<jasmine.Spy>httpClientService.get).and.returnValue(response);
    (<jasmine.Spy>responseObject.json).and.returnValue(expectedRooms);

    fixture.getRooms(eventId).subscribe((rooms) => {
      expect(rooms).toEqual(expectedRooms);
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

    let expectedRoom = Room.fromJson({});
    (<jasmine.Spy>httpClientService.put).and.returnValue(response);
    (<jasmine.Spy>responseObject.json).and.returnValue(expectedRoom);

    fixture.updateRoom(eventId, roomId, room).subscribe((updatedRoom: Room) => {
      expect(updatedRoom).toEqual(expectedRoom);
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

  describe('#getUnassignedGroups', () => {
    it('should return unassigned groups', () => {
      let group: Group[] = [
        new Group(),
        new Group()
      ];
      group[0].Group_Name = '12345';
      group[1].Group_Name = '67890';

      (<jasmine.Spy>httpClientService.get).and.returnValue(response);
      (<jasmine.Spy>responseObject.json).and.returnValue(group);

      let result = fixture.getUnassignedGroups(231);
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/events/231/groups/unassigned`);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
      result.subscribe((c) => {
        expect(c[0].Group_Name).toEqual(group[0]['Group_Name']);
      });
    });
  });

  describe('#reprint', () => {
    it('should reprint name tag', () => {
      let participantEventId = 123;

      (<jasmine.Spy>httpClientService.post).and.returnValue(response);
      (<jasmine.Spy>responseObject.json).and.returnValue({});

      fixture.reprint(participantEventId).subscribe((res) => {
        expect(httpClientService.post).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/signin/participant/${participantEventId}/print`, {});
      });
    });
  });

  describe('#findFamilies', () => {
    it('should return head of household contacts for the last name', () => {
      let contacts: Contact[] = [
        new Contact(),
        new Contact()
      ];
      contacts[0].HouseholdId = 12;
      contacts[1].HouseholdId = 1;

      (<jasmine.Spy>httpClientService.get).and.returnValue(response);
      (<jasmine.Spy>responseObject.json).and.returnValue(contacts);

      let result = fixture.findFamilies('dustin');
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/findFamily?search=dustin`);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
      result.subscribe((c) => {
        expect(c[0].HouseholdId).toEqual(contacts[0]['HouseholdId']);
      });
    });
  });

  describe('#getChildrenByHousehold', () => {
    it('should return children in household', () => {
      const householdId = 4312;
      let eventParticipants = new EventParticipants();
      eventParticipants.Participants = [new Child(), new Child()];
      eventParticipants.Participants[0].ParticipantId = 1;
      eventParticipants.Participants[1].ParticipantId = 3;
      eventParticipants.CurrentEvent = new Event();
      eventParticipants.CurrentEvent.EventId = 43224;
      (<jasmine.Spy>httpClientService.get).and.returnValue(response);
      (<jasmine.Spy>responseObject.json).and.returnValue(eventParticipants);

      let result = fixture.getChildrenByHousehold(householdId);
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/signin/children/household/${householdId}`);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
      result.subscribe((r) => {
        expect(r.Participants[0].ParticipantId).toEqual(eventParticipants.Participants[0].ParticipantId);
        expect(r.Participants[1].ParticipantId).toEqual(eventParticipants.Participants[1].ParticipantId);
      });
    });
  });

});
