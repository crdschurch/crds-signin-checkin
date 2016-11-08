/* tslint:disable:max-line-length */

import { Observable } from 'rxjs/Observable';
import { ApiService } from '.';
import { Response, ResponseOptions } from '@angular/http';
import { Event } from '../models';

describe('ApiService', () => {
  let fixture: ApiService;
  let httpClientServiceStub: any;
  let setupServiceStub: any;
  let responseOptions: ResponseOptions;
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

  fdescribe('ApiService', () => {

    describe('#getEvents', () => {
      const responseData = eventsData;
      beforeEach(() => {
        setupServiceStub = {
          getMachineDetailsConfigCookie() {
              return '1';
          }
        };
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
        setupServiceStub = {
          getMachineDetailsConfigCookie() {
              return '1';
          }
        };
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
          console.log("hi")
          console.log(res)
          console.log(responseObject)
          expect(res).toEqual(Event.fromJson(responseObject));
        });
      });
    });
  });

});
