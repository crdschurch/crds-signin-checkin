import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { RequestOptions, URLSearchParams } from '@angular/http';

import * as moment from 'moment';
import '../../rxjs-operators';
import { HttpClientService, SetupService } from '.';
import { Congregation, Event, Group } from '../models';

@Injectable()
export class ApiService {
  cachedEvent: Event;

  constructor(private http: HttpClientService, private setupService: SetupService) {}

  getEventMaps(eventId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/maps`;
    return this.http.get(url)
                    .map(res => Event.fromJsons(res.json()))
                    .catch(this.handleError);
  }

  getEvent(eventId: string, fromCache = false) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}`;
    if (fromCache && this.cachedEvent && (this.cachedEvent.EventId === parseInt(eventId, 10))) {
      return Observable.of(this.cachedEvent);
    }
    return this.http.get(url)
                    .map(res => {
                      const e = Event.fromJson(res.json());
                      this.cachedEvent = e;
                      return e;
                    })
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
                    .map(res => {
                      let events = res.json();
                      return events.sort((a: Event, b: Event) => {
                        return a.EventStartDate.localeCompare(b.EventStartDate);
                      });
                    })
                    .catch(this.handleError);
  }

  getEventTemplates(site?: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/templates`;
    if (!site) { site = this.getSite(); }
    let options = new RequestOptions({
        search: new URLSearchParams(`site=${site}`)
    });
    return this.http.get(url, options)
                    .map(res => {
                      let events = res.json();
                      return events.sort((a: Event, b: Event) => {
                        return a.EventStartDate.localeCompare(b.EventStartDate);
                      });
                    })
                    .catch(this.handleError);
  }

  getGradeGroups(eventId = null) {
    let url = `${process.env.ECHECK_API_ENDPOINT}/grade-groups`;
    url += (eventId === null ? '' : `?eventId=${eventId}`);

    return this.http.get(url)
                    .map(res => { return (<Group[]>res.json()).map(r => Group.fromJson(r)); })
                    .catch(this.handleError);
  }

  getSites() {
    const url = `${process.env.ECHECK_API_ENDPOINT}/sites`;
    return this.http.get(url)
                    .map(res => {
                      let allCongregations = (<Congregation[]>res.json()).map(r => Congregation.fromJson(r));
                      // remove "I do not attend Crossroads" record and "Anywhere"
                      return allCongregations.filter(e => {
                        return e.CongregationId !== 2 && e.CongregationId !== 15;
                      });
                    })
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
