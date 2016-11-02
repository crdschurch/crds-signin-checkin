/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { SetupService } from './setup.service';
import { HttpClientService } from '../shared/services/http-client.service';
import { Http, RequestOptions, Headers, Response, ResponseOptions } from '@angular/http';
import { MockConnection, MockBackend } from '@angular/http/testing';
import { CookieService, CookieOptions } from 'angular2-cookie/core';

describe('SetupService', () => {
  let fixture: SetupService;
  let httpClientService: HttpClientService;
  let http: Http;
  let options: RequestOptions;
  let backend: MockBackend;
  let cookieService: CookieService;
  let cookie: any;
  let thisCookieService: any;
  const machineIdStub = '1991653a-ddb8-47fd-9d3a-86761506fa4f';
  const machineConfigStub = { 'KioskConfigId': 1, 'KioskIdentifier': '1991653a-ddb8-47fd-9d3a-86761506fa4f', 'KioskName': 'Test Kiosk 1 Name', 'KioskDescription': 'Test Kiosk 1 Desc', 'KioskTypeId': 1, 'LocationId': 3, 'CongregationId': 1, 'RoomId': 1984, 'StartDate': '2016-10-27T00:00:00', 'EndDate': null };

  describe('SetupService', () => {

    beforeEach(() => {
      cookieService = new CookieService(new CookieOptions());
      cookieService.putObject('machine_config_id', machineIdStub);
      cookieService.putObject('machine_config_details', machineConfigStub);
      thisCookieService = cookieService;
      backend = new MockBackend();
      options = new RequestOptions();
      options.headers = new Headers();
      http = new Http(backend, options);
      cookie = thisCookieService;
      httpClientService = new HttpClientService(http, cookie);
      fixture = new SetupService(httpClientService, cookie);
    });

    it('should get this machine\'s configuration from server if server return configuration', (done) => {
      fixture.getThisMachineConfiguration().subscribe(
          machineConfig => {
            expect(machineConfig).toEqual(machineConfigStub);
            done()
          },
          error => {}
        );
    });

    beforeEach(() => {
      cookieService = new CookieService(new CookieOptions());
      cookieService.putObject('machine_config_id', undefined);
      cookieService.putObject('machine_config_details', {});
      thisCookieService = cookieService;
      backend = new MockBackend();
      options = new RequestOptions();
      options.headers = new Headers();
      http = new Http(backend, options);
      cookie = thisCookieService;
      httpClientService = new HttpClientService(http, cookie);
      fixture = new SetupService(httpClientService, cookie);
    });

    it('should catch an error if no configuration returned from server', () => {
      fixture.getThisMachineConfiguration().subscribe(
          machineConfig => {},
          error => {
            expect(error).toBeDefined();
          }
        );
    });

  });
});
