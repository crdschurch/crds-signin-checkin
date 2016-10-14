import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import '../rxjs-operators';
import { HttpClientService } from '../shared/services';

@Injectable()
export class AdminService {
  private url: string = '';

  constructor(private http: HttpClientService) {
  }

  getEvents() {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events`;
    return this.http.get(url)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  getRooms(eventId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms`;
    return this.http.get(this.url)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  getRoom(eventId: string, roomId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`;
    return this.http.get(this.url)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  updateRoom(eventId: string, roomId: string, body: any) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`;
    return this.http.put(this.url, body)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
