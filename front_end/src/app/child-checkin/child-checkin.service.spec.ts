/* tslint:disable:max-line-length */

import { TestBed } from '@angular/core/testing';
import { Observable } from 'rxjs';
import { RequestOptions } from '@angular/http';

import { EventParticipants } from '../shared/models/event-participants';
import { Child } from '../shared/models/child';
import { Event } from '../shared/models/event';
import { ChildCheckinService } from './child-checkin.service';
import { HttpClientService } from '../shared/services/http-client.service';

describe('ChildCheckinService', () => {
  let fixture: ChildCheckinService;
  let httpClientService: any;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [
        HttpClientService,
      ],
    });

    httpClientService = jasmine.createSpyObj('httpClientService', ['post', 'put', 'get']);
    fixture = new ChildCheckinService(httpClientService);
  });

  describe('#getChildrenForRoom', () => {
    it('should get children', () => {
      let output = new EventParticipants();
      output.CurrentEvent = new Event();
      output.Participants = [
        new Child()
      ];
      output.CurrentEvent.EventId = 6789;
      output.Participants[0].ContactId = 9876;

      httpClientService.get.and.callFake((url: string, data: any, opts: any) => {
        return Observable.of({ json: () => {
          return JSON.stringify(output);
        }});
      });

      let result = fixture.getChildrenForRoom(1820);
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/checkin/children/1820`);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
    });
  });

  describe('#checkInChildren', () => {
    it('should check in children to backend', () => {
      let input = new Child();
      input.ContactId = 54321;

      let output = new Child();
      output.ContactId = 9876;

      httpClientService.put.and.callFake((url: string, data: Child, opts: RequestOptions) => {
        return Observable.of({ json: () => {
          return JSON.stringify(output);
        }});
      });

      let result = fixture.checkInChildren(input);
      expect(httpClientService.put).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/checkin/event/participant`, input);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
    });
  });

  describe('#getChildByCallNumber', () => {
    it('get child from backend', () => {
      let child = new Child();
      child.ContactId = 54321;
      httpClientService.get.and.callFake((url: string, data: Child, opts: RequestOptions) => {
        return Observable.of({ json: () => {
          return JSON.stringify(child);
        }});
      });
      let result = fixture.getChildByCallNumber(123, '456', 789);
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/checkin/events/123/child/456/rooms/789`);
      expect(result).toBeDefined();
      result.subscribe((p) => {
        expect(p).toEqual(jasmine.any(Child));
      });
    });
    describe('error scenario', () => {
      it('get error string back', () => {
        httpClientService.get.and.callFake((url: string, data: Child, opts: RequestOptions) => {
          return Observable.throw('err');
        });
        let result = fixture.getChildByCallNumber(123, '456', 789);
        result.subscribe((p) => {}, error => {
          expect(error).toEqual('err');
        });
      });
    });
  });

  describe('#overrideChildIntoRoom', () => {
    it('override a child into a room', () => {
      let child = new Child();
      child.EventParticipantId = 54321;
      httpClientService.put.and.callFake((url: string, data: Child, opts: RequestOptions) => {
        return Observable.of();
      });
      let result = fixture.overrideChildIntoRoom(child, 123, 555);
      expect(httpClientService.put).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/checkin/events/123/child/54321/rooms/555/override`, {});
      expect(result).toBeDefined();
    });
    describe('error scenarios', () => {
      it('should return error string', () => {
        httpClientService.put.and.callFake(() => {
          return Observable.throw('err');
        });
        let result = fixture.overrideChildIntoRoom(new Child(), 123, 555);
        result.subscribe((r) => {
            expect(r).not.toBeDefined();
          }, error => {
            expect(error).toBeDefined();
        });
      });
    });
  });
});
