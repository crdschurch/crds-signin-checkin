/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { HttpClientService } from './http-client.service';
import { Http, RequestOptions, Headers, Response, ResponseOptions } from '@angular/http';
import { MockConnection, MockBackend } from '@angular/http/testing';
import {CookieService, CookieOptions} from 'angular2-cookie/core';
import { UserService } from './user.service';
import { User, MachineConfiguration } from '../models';

describe('HttpClientService', () => {
  let fixture: HttpClientService;
  let http: Http;
  let options: RequestOptions;
  let backend: MockBackend;
  let responseObject: any;
  let cookie: CookieService;
  let userService: UserService;
  let user: User;

  beforeEach(() => {
    user = new User();
    user.token = 'tok123';
    user.refreshToken = 'ref123';

    backend = new MockBackend();
    options = new RequestOptions();
    options.headers = new Headers();
    http = new Http(backend, new RequestOptions());
    cookie = new CookieService(new CookieOptions());
    userService = jasmine.createSpyObj<UserService>('userService', ['getUser', 'setUser', 'logOut', 'isLoggedIn']);
    fixture = new HttpClientService(http, cookie, userService);
  });

  describe('get method', () => {
    responseObject = { 'test': 123 };
    let requestHeaders: Headers;
    let requestUrl: string;
    beforeEach(() => {
      (<jasmine.Spy>userService.getUser).and.returnValue(user);
      backend.connections.subscribe((connection: MockConnection) => {
        requestHeaders = connection.request.headers;
        requestUrl = connection.request.url;

        let responseOptions = new ResponseOptions();
        responseOptions.headers = new Headers();
        responseOptions.headers.append('Authorization', 'tok456');
        responseOptions.headers.append('RefreshToken', 'ref456');
        connection.mockRespond(new Response(new ResponseOptions({
          body: responseObject,
          headers: responseOptions.headers
        })));
      });
    });

    afterEach(() => {
      backend.resolveAllConnections();
      backend.verifyNoPendingRequests();
    });

    it('should call http.get() with proper URL and use existing RequestOptions', () => {
      let response = fixture.get('/test/123', options);
      response.subscribe((res: Response) => {
        expect(res.json()).toEqual(responseObject);
        expect(requestHeaders).toBeDefined();
        expect(requestHeaders.get('Content-Type')).toEqual('application/json');
        expect(requestHeaders.get('Accept')).toEqual('application/json, text/plain, */*');
        expect(requestHeaders.get('Crds-Api-Key')).toEqual(process.env.ECHECK_API_TOKEN);
        expect(requestHeaders.get('Authorization')).toEqual('tok123');
        expect(requestHeaders.get('RefreshToken')).toEqual('ref123');
        expect(requestUrl).toEqual('/test/123');

        expect(userService.getUser).toHaveBeenCalled();
        expect(userService.setUser).toHaveBeenCalledWith(user);
        expect(user.token).toEqual('tok456');
        expect(user.refreshToken).toEqual('ref456');
      });
    });

    it('should call http.get() with proper URL and create new RequestOptions', () => {
      options.headers.set('this-one', 'should not be here');
      let response = fixture.get('/test/123');
      response.subscribe((res: Response) => {
        expect(res.json()).toEqual(responseObject);
        expect(requestHeaders).toBeDefined();
        expect(requestHeaders).not.toEqual(options.headers);
        expect(requestHeaders.get('Content-Type')).toEqual('application/json');
        expect(requestHeaders.get('Accept')).toEqual('application/json, text/plain, */*');
        expect(requestHeaders.get('Crds-Api-Key')).toEqual(process.env.ECHECK_API_TOKEN);
        expect(requestHeaders.has('this-one')).toBeFalsy();
        expect(requestUrl).toEqual('/test/123');
      });
    });

    it('should set authentication token if sent in response', () => {
      responseObject.userToken = '98765';
      let response = fixture.get('/events');
      response.subscribe((r) => {
        expect(userService.setUser).toHaveBeenCalled();
      });
    });

    it('should send authentication token if logged in', () => {
      user.token = undefined;
      user.refreshToken = undefined;
      let response = fixture.get('/test/123');
      response.subscribe((r) => {
        let r2 = fixture.get('/test/123');
        r2.subscribe(() => {
          expect(requestHeaders.has('Authorization')).toBeTruthy();
          expect(requestHeaders.get('Authorization')).toEqual('tok456');
          expect(requestHeaders.has('RefreshToken')).toBeTruthy();
          expect(requestHeaders.get('RefreshToken')).toEqual('ref456');
        });
      });
    });

    it('should send site id and kiosk id if they exist', () => {
      let machineConfig = new MachineConfiguration();
      machineConfig.CongregationId = 678;
      machineConfig.KioskIdentifier = '12345678';

      spyOn(cookie, 'getObject').and.returnValue(machineConfig);
      let response = fixture.get('/test/123');
      response.subscribe((r) => {
        expect(requestHeaders.has('Crds-Site-Id')).toBeTruthy();
        expect(requestHeaders.get('Crds-Site-Id')).toEqual(`${machineConfig.CongregationId}`);
        expect(requestHeaders.has('Crds-Kiosk-Identifier')).toBeTruthy();
        expect(requestHeaders.get('Crds-Kiosk-Identifier')).toEqual(machineConfig.KioskIdentifier);
      });
    });
  });
});
