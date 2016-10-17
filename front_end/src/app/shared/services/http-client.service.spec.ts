/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { HttpClientService } from './http-client.service';
import { Http, RequestOptions, Headers, Response, ResponseOptions } from '@angular/http';
import { MockConnection, MockBackend } from '@angular/http/testing';
import {CookieService, CookieOptions} from 'angular2-cookie/core';

describe('HttpClientService', () => {
  let fixture: HttpClientService;
  let http: Http;
  let options: RequestOptions;
  let backend: MockBackend;
  let responseObject: any;
  let cookie: CookieService;

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
    fixture = new HttpClientService(http, cookie);
  });

  describe('get method', () => {
    responseObject = { 'test': 123 };
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
      fixture.logOut();
    });

    it('should call http.get() with proper URL and use existing RequestOptions', () => {
      options.headers.set('this', 'should still be here');
      let response = fixture.get('/test/123', options);
      response.subscribe((res: Response) => {
        expect(res.json()).toEqual(responseObject);
        expect(requestHeaders).toBeDefined();
        expect(requestHeaders).toEqual(options.headers);
        expect(requestHeaders.get('Content-Type')).toEqual('application/json');
        expect(requestHeaders.get('Accept')).toEqual('application/json, text/plain, */*');
        expect(requestHeaders.get('Crds-Api-Key')).toEqual(process.env.ECHECK_API_TOKEN);
        expect(requestHeaders.get('this')).toEqual('should still be here');
        expect(requestUrl).toEqual('/test/123');
        expect(fixture.isLoggedIn()).toBeFalsy();
      });
    });

    it('should call http.get() with proper URL and create new RequestOptions', () => {
      options.headers.set('this', 'should not be here');
      let response = fixture.get('/test/123');
      response.subscribe((res: Response) => {
        expect(res.json()).toEqual(responseObject);
        expect(requestHeaders).toBeDefined();
        expect(requestHeaders).not.toEqual(options.headers);
        expect(requestHeaders.get('Content-Type')).toEqual('application/json');
        expect(requestHeaders.get('Accept')).toEqual('application/json, text/plain, */*');
        expect(requestHeaders.get('Crds-Api-Key')).toEqual(process.env.ECHECK_API_TOKEN);
        expect(requestHeaders.has('this')).toBeFalsy();
        expect(requestUrl).toEqual('/test/123');
        expect(fixture.isLoggedIn()).toBeFalsy();
      });
    });

    it('should set authentication token if sent in response', () => {
      responseObject.userToken = '98765';
      let response = fixture.get('/test/123');
      response.subscribe(() => {
        expect(fixture.isLoggedIn()).toBeTruthy();
      });
    });

    it('should send authentication token if logged in', () => {
      expect(fixture.isLoggedIn()).toBeFalsy();
      responseObject.userToken = '98765';
      let response = fixture.get('/test/123');
      response.subscribe(() => {
        expect(fixture.isLoggedIn()).toBeTruthy();
        let r2 = fixture.get('/test/123');
        r2.subscribe(() => {
          expect(requestHeaders.has('Authorization')).toBeTruthy();
          expect(requestHeaders.get('Authorization')).toEqual('98765');
        });
      });
    });
  });
});
