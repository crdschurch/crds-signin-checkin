import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { RequestOptions, URLSearchParams } from '@angular/http';

import '../rxjs-operators';
import { HttpClientService } from '../shared/services';
import { Event } from './events/event';
import { Room } from './rooms/room';
import * as moment from 'moment';
import { Room } from './rooms/room'
import { Group } from './rooms/group'

@Injectable()
export class AdminService {

  constructor(private http: HttpClientService) {
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
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`;
    return this.http.get(url)
                    .map(res => Room.fromJson(res.json()))
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
