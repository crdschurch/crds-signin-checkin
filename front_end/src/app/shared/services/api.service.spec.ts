/* tslint:disable:max-line-length */

import { Observable } from 'rxjs/Observable';
import { ApiService } from '.';
import { Response, ResponseOptions } from '@angular/http';
import { Event } from '../models';

let fixture: ApiService;
let httpClientServiceStub: any;
let responseOptions: ResponseOptions;
let setupServiceStub: any = {
  getMachineDetailsConfigCookie() { return '1'; }
};
const eventData = {
    'EventId': 4525328,
    'EventTitle': 'John Test Event 10-11-16',
    'EventStartDate': '2016-10-29T09:30:00',
    'EventType': 'Service - Oakley',
    'EventSite': 'Oakley'
};
const eventsData = [{
    'EventId': 4525323,
    'EventTitle': 'John Test Event 10-11-16',
    'EventStartDate': '2016-10-29T09:30:00',
    'EventType': 'Service - Oakley',
    'EventSite': 'Oakley'
}, {
    'EventId': 4525342,
    'EventTitle': '(t) Jim Test Service',
    'EventStartDate': '2016-10-29T16:30:00',
    'EventType': 'Service - Oakley',
    'EventSite': 'Oakley'
}];

describe('ApiService', () => {

  describe('#getEvents', () => {
    const responseData = eventsData;
    beforeEach(() => {
      httpClientServiceStub = {
        get() {
          responseOptions = new ResponseOptions({ body: responseData });
          return Observable.of(new Response(responseOptions));
        }
      };
      fixture = new ApiService(httpClientServiceStub, setupServiceStub);
    });
    it('should successfully get list of events', () => {
      let responseObject = responseData;
      fixture.getEvents('10-11-16', '10-15-16').subscribe((res: Response) => {
        expect(res).toEqual(responseObject);
      });
    });
  });

  describe('#getEvent', () => {
    const responseData = eventData;
    beforeEach(() => {
      httpClientServiceStub = {
        get() {
          responseOptions = new ResponseOptions({ body: responseData });
          return Observable.of(new Response(responseOptions));
        }
      };
      fixture = new ApiService(httpClientServiceStub, setupServiceStub);
    });
    it('should successfully get event', () => {
      let responseObject = responseData;
      fixture.getEvent('453').subscribe((res: Response) => {
        expect(res).toEqual(Event.fromJson(responseObject));
      });
    });
  });

  describe('#getEventMaps', () => {
    const responseData = eventsData;
    beforeEach(() => {
      httpClientServiceStub = {
        get() {
          responseOptions = new ResponseOptions({ body: responseData });
          return Observable.of(new Response(responseOptions));
        }
      };
      fixture = new ApiService(httpClientServiceStub, setupServiceStub);
    });
    it('should successfully get event maps', () => {
      let responseObject = responseData;
      fixture.getEvents('453').subscribe((res: Response) => {
        expect(res).toEqual(responseObject);
      });
    });
  });

});
