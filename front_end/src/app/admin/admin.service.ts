import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import '../rxjs-operators';
import { HttpClientService } from '../shared/services';
import { Room } from '../shared/models';

@Injectable()
export class AdminService {
  private roomGroupsUpdateEmitter: EventEmitter<Room>;
  private roomGroupsUpdateObserver: Observable<Room>;
  site: number;

  constructor(private http: HttpClientService) {
    this.configureUpdateRoomGroupsEmitterAndObserver();
  }

  getRooms(eventId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms`;
    return this.http.get(url)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  getBumpingRooms(eventId: string, roomId: string) {
    // const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}/bumping`;
    // return this.http.get(url)
    //                 .map(res => {return data})
    //                 .catch(this.handleError);
    // TODO: remove when backend available
    const data = [
      { EventRoomId: "456", RoomId: "456", EventId: "456", RoomName: "456", RoomNumber: "456", AllowSignIn: true, Volunteers: 3, Capacity: 4, SignedIn: 5, CheckedIn: 6, Label: "label123", BumpingRuleId: 897, BumpingRulePriority: 2 },
      { EventRoomId: "888", RoomId: "888", EventId: "888", RoomName: "888", RoomNumber: "888", AllowSignIn: true, Volunteers: 3, Capacity: 4, SignedIn: 5, CheckedIn: 6, Label: "label123", BumpingRuleId: 897, BumpingRulePriority: 4 },
      { EventRoomId: "987", RoomId: "987", EventId: "987", RoomName: "987", RoomNumber: "987", AllowSignIn: true, Volunteers: 3, Capacity: 4, SignedIn: 5, CheckedIn: 6, Label: "label123", BumpingRuleId: 897, BumpingRulePriority: 3 },
      { EventRoomId: "123", RoomId: "123", EventId: "123", RoomName: "123", RoomNumber: "123", AllowSignIn: true, Volunteers: 3, Capacity: 4, SignedIn: 5, CheckedIn: 6, Label: "label123", BumpingRuleId: 897, BumpingRulePriority: 1 },
      { EventRoomId: "789", RoomId: "789", EventId: "789", RoomName: "789", RoomNumber: "789", AllowSignIn: true, Volunteers: 3, Capacity: 4, SignedIn: 5, CheckedIn: 6, Label: "label123" },
    ]
    return Observable.of(data);
  }

  updateRoom(eventId: string, roomId: string, body: Room) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`;
    return this.http.put(url, body)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  getRoomGroups(eventId: string, roomId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}/groups`;
    return this.http.get(url)
                    .map(res => Room.fromJson(res.json()))
                    .catch(this.handleError);
  }

  updateRoomGroups(eventId: string, roomId: string, body: Room) {
    body.EventId = eventId;
    body.RoomId = roomId;
    this.roomGroupsUpdateEmitter.emit(body);
  }

  importEvent(destinationEventId: number, sourceEventId: number): Observable<Room[]> {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${destinationEventId}/import/${sourceEventId}`;
    return this.http.put(url, null, null)
                    .map(res => { (<any[]>res.json()).map(r => Room.fromJson(r)); })
                    .catch(this.handleError);
  }

  private updateRoomGroupsInternal(room: Room) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${room.EventId}/rooms/${room.RoomId}/groups`;
    return this.http.put(url, room)
                    .map(res => Room.fromJson(res.json()))
                    .catch(this.handleError);
  }

  private configureUpdateRoomGroupsEmitterAndObserver() {
    // Create an emitter to use when sending updates to the room
    this.roomGroupsUpdateEmitter = new EventEmitter<Room>();

    // Setup an observer on the emitter, and set it to debounce for 2 seconds.
    // This prevents the frontend from sending a backend update if multiple
    // age ranges or grades are selected quickly.
    this.roomGroupsUpdateObserver =
      this.roomGroupsUpdateEmitter.map(room => room).debounceTime(2000);

    // Subscribe to the debounced event - now actually send the update to
    // the backend.
    // TODO - Should handle the response, and notify the user of success or failure
    // TODO - Should have some sort of processing state while the update is running, since it can take several seconds to complete
    this.roomGroupsUpdateObserver.subscribe(room => {
      this.updateRoomGroupsInternal(room);
    });
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
