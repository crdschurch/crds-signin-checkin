/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { SignInService } from './sign-in.service';
import { HttpClientService } from '../../shared/services/http-client.service';
import { Http, RequestOptions, Headers, Response, ResponseOptions } from '@angular/http';
import { MockConnection, MockBackend } from '@angular/http/testing';
import {CookieService, CookieOptions} from 'angular2-cookie/core';

describe('SignInService', () => {
  let fixture: SignInService;
  let httpClientService: HttpClientService;
  let http: Http;
  let options: RequestOptions;
  let backend: MockBackend;
  let responseObject: any;
  let cookie: CookieService;
  let body: any;
  let requestHeaders: Headers;
  let requestUrl: string;

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
    fixture = new SignInService(httpClientService);
    body = { email: 'test@test.com', password: 'password123' };
  });

  describe('when logging in', () => {
    afterEach(() => {
      backend.resolveAllConnections();
      backend.verifyNoPendingRequests();
      httpClientService.logOut();
    });

    describe('with successful credentials', () => {
      beforeEach(() => {
        backend.connections.subscribe((connection: MockConnection) => {
          requestHeaders = connection.request.headers;
          requestUrl = connection.request.url;
          const headers = new Headers();
          headers.append('Authorization', '98765');
          headers.append('RefreshToken', 'refresh-8885');
          connection.mockRespond(new Response(new ResponseOptions({
            body: responseObject,
            headers: headers
          })));
        });
      });

      it('should successfully login a user', () => {
        responseObject = { userToken: 'userToken1', refreshToken: 'refreshToken1', roles: ['123', '456'] };
        let response = http.post( `${process.env.ECHECK_API_ENDPOINT}/authenticate`, body);
        let login = fixture.logIn(body.email, body.password);
        response.subscribe((res: Response) => {
          expect(res.json()).toEqual(responseObject);
          expect(requestHeaders).toBeDefined();
          expect(httpClientService.isLoggedIn()).toBeTruthy();
          expect(httpClientService.hasRefreshToken()).toBeTruthy();
        });
      });
    });

    describe('with bad credentials', () => {
      beforeEach(() => {
        backend.connections.subscribe((connection: MockConnection) => {
          requestHeaders = connection.request.headers;
          requestUrl = connection.request.url;
          const headers = new Headers();
          // do not add successful auth, refreshtoken headers
          connection.mockRespond(new Response(new ResponseOptions({
            body: responseObject,
            headers: headers
          })));
        });
      });

      it('should not login a user', () => {
        responseObject = { userToken: undefined };
        let response = http.post( `${process.env.ECHECK_API_ENDPOINT}/authenticate`, body);
        let login = fixture.logIn(body.email, body.password);
        response.subscribe((res: Response) => {
          expect(httpClientService.isLoggedIn()).toBeFalsy();
          expect(httpClientService.hasRefreshToken()).toBeFalsy();
        });
      });
    });

  });
});
