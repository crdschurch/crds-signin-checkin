import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import '../rxjs-operators';
import { HttpClientService, SetupService } from '../shared/services';
import { Room } from '../shared/models';

@Injectable()
export class AdminService {
  private roomGroupsUpdateEmitter: EventEmitter<Room>;
  private roomGroupsUpdateObserver: Observable<Room>;
  site: number;

  constructor(private http: HttpClientService, private setupService: SetupService) {
    this.configureUpdateRoomGroupsEmitterAndObserver();
  }

  getRooms(eventId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms`;
    return this.http.get(url)
                    .map(res => res.json())
                    .catch(this.handleError);
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
