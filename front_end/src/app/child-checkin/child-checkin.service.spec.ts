/* tslint:disable:max-line-length */

import { TestBed } from '@angular/core/testing';
import { Observable } from 'rxjs';
import { RequestOptions } from '@angular/http';

import { EventParticipants } from '../shared/models/eventParticipants';
import { Child } from '../shared/models/child';
import { Event } from '../admin/events/event';
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
});
