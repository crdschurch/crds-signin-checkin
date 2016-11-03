/* tslint:disable:max-line-length */

import { HomeComponent } from './home.component';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import { CookieService, CookieOptions } from 'angular2-cookie/core';

let fixture: HomeComponent;
let router: Router;
let cookieService: CookieService;
let childCheckinRedirectSpy: any;
let setupErrorRedirectSpy: any;
let machineIdConfig: any;
let thisMachineConfig: any;

let setupServiceStub: any = {
  getMachineIdConfigCookie() {
    return machineIdConfig;
  },
  getThisMachineConfiguration() {
    return thisMachineConfig;
  }
};

describe('Home Component', () => {

  afterEach(() => {
    cookieService.removeAll();
  });

  describe('where there is a valid machine configuration cookie present', () => {

    beforeEach(() => {
      machineIdConfig = 'valid-cookie';
      thisMachineConfig = Observable.of({'MachineId': '123'});
      cookieService = new CookieService(new CookieOptions());
      cookieService.putObject('machine_config_id', '1991653a-ddb8-47fd-9d3a-86761506fa4f');
      cookieService.putObject('machine_config_details', { 'KioskConfigId': 1, 'KioskIdentifier': '1991653a-ddb8-47fd-9d3a-86761506fa4f', 'KioskName': 'Test Kiosk 1 Name', 'KioskDescription': 'Test Kiosk 1 Desc', 'KioskTypeId': 1, 'LocationId': 3, 'CongregationId': 1, 'RoomId': 1984, 'StartDate': '2016-10-27T00:00:00', 'EndDate': null });
      fixture = new HomeComponent(router, cookieService, setupServiceStub);
      childCheckinRedirectSpy = spyOn(fixture, 'goToChildSignin');
    });

    it('should redirect to child sign in when cookie is type = Sign In', () => {
      expect(cookieService.getObject('machine_config_details')['KioskTypeId']).toEqual(1);
      fixture.ngOnInit();
      expect(childCheckinRedirectSpy).toHaveBeenCalled();
    });

  });

  describe('where there is not an invalid machine configuration cookie present', () => {

    beforeEach(() => {
      machineIdConfig = undefined;
      thisMachineConfig = undefined;
      cookieService = new CookieService(new CookieOptions());
      fixture = new HomeComponent(router, cookieService, setupServiceStub);
      childCheckinRedirectSpy = spyOn(fixture, 'goToChildSignin');
    });

    it('should redirect to child sign in when cookie is type = Sign In', () => {
      fixture.ngOnInit();
      expect(childCheckinRedirectSpy).not.toHaveBeenCalled();
    });

  });

  describe('where there is an invalid cookie present', () => {

    beforeEach(() => {
      machineIdConfig = 'bad-cookie';
      thisMachineConfig = Observable.throw('invalid machine config');
      cookieService = new CookieService(new CookieOptions());
      fixture = new HomeComponent(router, cookieService, setupServiceStub);
      setupErrorRedirectSpy = spyOn(fixture, 'goToSetupError');
    });

    it('should redirect to child sign in when cookie is type = Sign In', () => {
      fixture.ngOnInit();
      expect(setupErrorRedirectSpy).toHaveBeenCalled();
    });

  });


});
