/* tslint:disable:max-line-length */

import { Observable } from 'rxjs/Observable';
import { ApiService } from '.';
import { Response, ResponseOptions } from '@angular/http';
import { Congregation, Event, Group } from '../models';

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
      let r = responseData;
      fixture.getEvents('10-11-16', '10-15-16').subscribe((res: Response) => {
        expect(res).toEqual(r);
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
      let r = responseData;
      fixture.getEvent('453').subscribe((res: Response) => {
        expect(res).toEqual(Event.fromJson(r));
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
      fixture.getEventMaps('453').subscribe((res) => {
        expect(res.length).toEqual(2);
        expect(res[0].EventId).toEqual(4525323);
        expect(res[1].EventId).toEqual(4525342);
      });
    });
  });

  describe('#getGradeGroups', () => {
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
    it('should return list of rooms imported', () => {
      let groups: Group[] = [
        new Group(),
        new Group()
      ];
      groups[0].Id = 12345;
      groups[1].Id = 67890;

      fixture.getGradeGroups().subscribe((res) => {
        expect(res).toBeDefined();
      });
    });
  });

  describe('#getSites', () => {
    let responseData = [new Congregation(), new Congregation()];
    responseData[0].CongregationId = 222;
    responseData[1].CongregationId = 333;
    beforeEach(() => {
      httpClientServiceStub = {
        get() {
          responseOptions = new ResponseOptions({ body: responseData });
          return Observable.of(new Response(responseOptions));
        }
      };
      fixture = new ApiService(httpClientServiceStub, setupServiceStub);
    });
    it('should successfully get all sites', () => {
      let r = responseData;
      fixture.getSites().subscribe((res: Response) => {
        expect(res[0].CongregationId).toEqual(responseData[0].CongregationId);
        expect(res[1].CongregationId).toEqual(responseData[1].CongregationId);
      });
    });
  });

});
