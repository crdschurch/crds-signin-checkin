import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import '../../rxjs-operators';
import { HttpClientService } from '../../shared/services';

@Injectable()
export class EventsService {
  private url: string = '';

  constructor(private http: HttpClientService) {
    this.url = `${process.env.ECHECK_API_ENDPOINT}/checkinevents`;
  }

  getAll() {
    console.log("getAll 1")
    return this.http.get(this.url)
                    // .map(res => res.json())
                    // .map(res => res._body.json())
                    // .map(res => console.log(res, res._body))
                    // .map(res => console.log(res, res.json()))
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
