/* tslint:disable:max-line-length */

import { TestBed } from '@angular/core/testing';
import { Http, RequestOptions, Headers, Response, ResponseOptions } from '@angular/http';
import { MockConnection, MockBackend } from '@angular/http/testing';
import { CookieService, CookieOptions } from 'angular2-cookie/core';

import { ChildSigninService } from './child-signin.service';
import { HttpClientService } from '../shared/services/http-client.service';

describe('ChildSigninService', () => {
  let fixture: ChildSigninService;
  let httpClientService: HttpClientService;
  let http: Http;
  let options: RequestOptions;
  let backend: MockBackend;
  let cookie: CookieService;
  let responseObject: any;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [
        HttpClientService
      ],
    });
    backend = new MockBackend();
    options = new RequestOptions();
    options.headers = new Headers();
    http = new Http(backend, options);
    cookie = new CookieService(new CookieOptions());
    httpClientService = new HttpClientService(http, cookie);
    fixture = new ChildSigninService(httpClientService);
  });

  describe('#getChildrenByPhoneNumber', () => {
    let requestHeaders: Headers;
    let requestUrl: string;

    beforeEach(() => {
      backend.connections.subscribe((connection: MockConnection) => {
        requestHeaders = connection.request.headers;
        requestUrl = connection.request.url;
        connection.mockRespond(new Response(new ResponseOptions({
          body: responseObject
        })));
      });
    });

    afterEach(() => {
      backend.resolveAllConnections();
      backend.verifyNoPendingRequests();
      httpClientService.logOut();
    });

    it('should get and then cache them when called again', () => {
      responseObject = http.get('assets/mock-data/available-children-get.json');
      let response = http.get( `${process.env.ECHECK_API_ENDPOINT}/signin/children/8128128123`);
      fixture.getChildrenByPhoneNumber('8128128123');

      response.subscribe((res: Response) => {
        expect(res.json()).toEqual(responseObject);
        expect(requestHeaders).toBeDefined();
      });
    });

    // it('should sign in children to backend', () => {
    //   responseObject = http.get('assets/mock-data/available-children-post.json');
    //   let response = http.post( `${process.env.ECHECK_API_ENDPOINT}/signin/children`, responseObject);
    //   // fixture.getChildrenByPhoneNumber('8128128123');
    //
    //   response.subscribe((res: Response) => {
    //     expect(res.json()).toEqual(responseObject);
    //     expect(requestHeaders).toBeDefined();
    //   });
    // });

  });
});
