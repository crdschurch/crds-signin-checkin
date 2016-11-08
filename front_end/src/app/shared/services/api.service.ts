import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { RequestOptions, URLSearchParams } from '@angular/http';

import * as moment from 'moment';
import '../../rxjs-operators';
import { HttpClientService, SetupService } from './';
import { Event } from '../models';

@Injectable()
export class ApiService {
  constructor(private http: HttpClientService, private setupService: SetupService) {
  }

  getEvent(eventId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}`;
    return this.http.get(url)
                    .map(res => Event.fromJson(res.json()))
                    .catch(this.handleError);
  }

  getEvents(startDate: any, endDate: any, site?: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events`;
    if (!site) { site = this.getSite(); }
    let formattedStartDate = moment(startDate, 'YYYY-MM-DD').format('YYYY-MM-DD');
    let formattedEndDate = moment(endDate, 'YYYY-MM-DD').format('YYYY-MM-DD');
    let options = new RequestOptions({
        search: new URLSearchParams(`site=${site}&startDate=${formattedStartDate}&endDate=${formattedEndDate}`)
    });
    return this.http.get(url, options)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  private getSite(): number {
    let setupCookie = this.setupService.getMachineDetailsConfigCookie();
    // default to Oakley if setup cookie is not present
    return setupCookie && setupCookie.CongregationId ? setupCookie.CongregationId : 1;
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
