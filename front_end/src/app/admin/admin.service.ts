import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { RequestOptions, URLSearchParams } from '@angular/http';

import * as moment from 'moment';
import '../rxjs-operators';
import { HttpClientService } from '../shared/services';
import { Event } from './events/event';
import { Room } from './rooms/room';

@Injectable()
export class AdminService {
  private roomGroupsUpdateEmitter: EventEmitter<Room>;
  private roomGroupsUpdateObserver: Observable<Room>;

  constructor(private http: HttpClientService) {
    this.configureUpdateRoomGroupsEmitterAndObserver();
  }

  getEvent(eventId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}`;
    return this.http.get(url)
                    .map(res => Event.fromJson(res.json()))
                    .catch(this.handleError);
  }

  getEvents(startDate: any, endDate: any, site: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events`;
    let formattedStartDate = moment(startDate, 'YYYY-MM-DD').format('YYYY-MM-DD');
    let formattedEndDate = moment(endDate, 'YYYY-MM-DD').format('YYYY-MM-DD');
    let options = new RequestOptions({
        search: new URLSearchParams(`site=${site}&startDate=${formattedStartDate}&endDate=${formattedEndDate}`)
    });
    return this.http.get(url, options)
                    .map(res => res.json())
                    .catch(this.handleError);
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
