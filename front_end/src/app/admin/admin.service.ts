import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import '../rxjs-operators';
import { HttpClientService } from '../shared/services';
import * as moment from 'moment';

import { RequestOptions, URLSearchParams } from '@angular/http';

@Injectable()
export class AdminService {

  constructor(private http: HttpClientService) {
  }

  getEvents(startDate: any, endDate: any, site: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events`;
    let formattedStartDate = moment(startDate).format('YYYY-MM-DD');
    let formattedEndDate = moment(endDate).format('YYYY-MM-DD');
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

  updateRoom(eventId: string, roomId: string, body: any) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`;
    return this.http.put(url, body)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  getRoomGroups(eventId: string, roomId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`;
    return this.http.get(url)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
