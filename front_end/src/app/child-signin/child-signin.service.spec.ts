/* tslint:disable:max-line-length */

import { TestBed } from '@angular/core/testing';
import { Observable } from 'rxjs';
import { RequestOptions } from '@angular/http';
import { Router } from '@angular/router';
import { Child, Event, EventParticipants} from '../shared/models';
import { ChildSigninService } from './child-signin.service';
import { HttpClientService } from '../shared/services/http-client.service';

describe('ChildSigninService', () => {
  let fixture: ChildSigninService;
  let httpClientService: any;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [
        HttpClientService,
      ],
    });

    httpClientService = jasmine.createSpyObj('httpClientService', ['post', 'put', 'get']);
    fixture = new ChildSigninService(httpClientService, router);
  });

  describe('#getChildrenByPhoneNumber', () => {
    it('should get and then cache them when called again', () => {
      let input = new EventParticipants();
      input.CurrentEvent = new Event();
      input.Participants = [
        new Child()
      ];
      input.CurrentEvent.EventId = 12345;
      input.Participants[0].ContactId = 54321;

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

      let result = fixture.getChildrenByPhoneNumber('8128128123');
      expect(httpClientService.get).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/signin/children/812-812-8123`);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
    });
  });

  describe('#signInChildren', () => {
    it('should sign in children to backend', () => {
      let input = new EventParticipants();
      input.CurrentEvent = new Event();
      input.Participants = [
        new Child()
      ];
      input.CurrentEvent.EventId = 12345;
      input.Participants[0].ContactId = 54321;

      let output = {
        CurrentEvent: {
          EventId: 6789
        },
        Participants: [
          {
            ContactId: 9876
          }
        ]
      };

      httpClientService.post.and.callFake((url: string, data: EventParticipants, opts: RequestOptions) => {
        return Observable.of({ json: () => {
          return output;
        }});
      });

      let result = fixture.signInChildren(input);
      expect(httpClientService.post).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/signin/children`, input);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
      result.subscribe((p) => {
        expect(p).toEqual(EventParticipants.fromJson(output));
      });
    });
  });

  describe('#printParticipantLabels', () => {
    it('should send proper request to print labels', () => {
      let input = new EventParticipants();
      input.CurrentEvent = new Event();
      input.Participants = [
        new Child()
      ];
      input.CurrentEvent.EventId = 12345;
      input.Participants[0].ContactId = 54321;

      let output = {
        CurrentEvent: {
          EventId: 6789
        },
        Participants: [
          {
            ContactId: 9876
          }
        ]
      };

      httpClientService.post.and.callFake((url: string, data: EventParticipants, opts: RequestOptions) => {
        return Observable.of({ json: () => {
          return output;
        }});
      });

      let result = fixture.printParticipantLabels(input);
      expect(httpClientService.post).toHaveBeenCalledWith(`${process.env.ECHECK_API_ENDPOINT}/signin/participants/print`, input);
      expect(result).toBeDefined();
      expect(result).toEqual(jasmine.any(Observable));
      result.subscribe((p) => {
        expect(p).toEqual(EventParticipants.fromJson(output));
      });
    });
  });

  describe('#reset', () => {
    it('should reset event participants and phone number', () => {
      httpClientService.get.and.callFake((url: string, data: any, opts: any) => {
        return Observable.of();
      });
      // set the phone number
      fixture.getChildrenByPhoneNumber('8128128123');
      fixture.reset();
      expect(fixture.getPhoneNumber()).toEqual('');
      expect(fixture.getEventParticipantsResults()).not.toBeDefined();
    });
  });
});
