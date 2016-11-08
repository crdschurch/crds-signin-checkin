/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { MachineConfiguration } from '../shared/models';
import { HttpClientService, SetupService } from '../shared/services';
import { Http, Response, RequestOptions, Headers, ResponseOptions } from '@angular/http';
import { MockConnection, MockBackend } from '@angular/http/testing';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { Observable } from 'rxjs/Observable';

describe('SetupService', () => {
  let fixture: SetupService;
  let httpClientService: any;
  let cookieService: CookieService;
  const machineIdStub = '1991653a-ddb8-47fd-9d3a-86761506fa4f';
  const machineConfigStub = {
    'KioskConfigId': 1,
    'KioskIdentifier': '1991653a-ddb8-47fd-9d3a-86761506fa4f',
    'KioskName': 'Test Kiosk 1 Name',
    'KioskDescription': 'Test Kiosk 1 Desc',
    'KioskTypeId': 1,
    'LocationId': 3,
    'CongregationId': 1,
    'RoomId': 1984,
    'StartDate': '2016-10-27T00:00:00',
    'EndDate': null
  };

  describe('when remotely getting machine configuration', () => {

    describe('and configuration is valid', () => {
      beforeEach(() => {
        cookieService = new CookieService(new CookieOptions());
        cookieService.putObject('machine_config_id', machineIdStub);
        cookieService.putObject('machine_config_details', machineConfigStub);
        httpClientService = {
          get(url: string) {
            let response = new ResponseOptions({ body: machineConfigStub });
            return Observable.of(new Response(response));
          }
        };
        fixture = new SetupService(httpClientService, cookieService);
      });

      it('should get this machine\'s configuration from server if server return configuration', () => {
        fixture.getThisMachineConfiguration().subscribe(
          machineConfig => {
            expect(machineConfig).toBeDefined();
            expect(machineConfig).toEqual(MachineConfiguration.fromJson(machineConfigStub));
          },
          error => {}
        );
      });

      describe('and configuration is invalid', () => {
        beforeEach(() => {
          cookieService = new CookieService(new CookieOptions());
          cookieService.putObject('machine_config_id', machineIdStub);
          cookieService.putObject('machine_config_details', machineConfigStub);
          httpClientService = {
            get(url: string) {
              let response = new ResponseOptions({ body: machineConfigStub });
              return Observable.throw('invalid config');
            }
          };
          fixture = new SetupService(httpClientService, cookieService);
        });

        it('should return an error', () => {
          fixture.getThisMachineConfiguration().subscribe(
            machineConfig => {},
            error => {
              expect(error).toBeDefined();
            }
          );
        });

      });

    });
  });

  describe('when setting machine id and machine details cookies', () => {

    beforeEach(() => {
      cookieService = new CookieService(new CookieOptions());
      cookieService.putObject('machine_config_id', undefined);
      cookieService.putObject('machine_config_details', undefined);
      fixture = new SetupService(undefined, cookieService);
    });

    it('should set and get a the machine id cookie', () => {
      const idCookieName = 'cookieName123';
      spyOn(cookieService, 'putObject').and.callThrough();
      fixture.setMachineIdConfigCookie(idCookieName);
      expect(fixture.getMachineIdConfigCookie()).toEqual(idCookieName);
      expect(cookieService.putObject).toHaveBeenCalledWith(MachineConfiguration.COOKIE_NAME_ID, idCookieName, jasmine.objectContaining({
        'expires': new Date(2038, 0, 19, 4, 14, 7)
      }));
    });

    it('should set and get a the machine id cookie', () => {
      const detailsCookieName = 'cookieName456';
      fixture.setMachineIdConfigCookie(detailsCookieName);
      expect(fixture.getMachineIdConfigCookie()).toEqual(detailsCookieName);
    });
  });

  // TODO set machine details
});
