/* tslint:disable:max-line-length */

import { HomeComponent } from './home.component';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { MachineConfiguration } from '../shared/models';

let fixture: HomeComponent;
let router: Router;
let cookieService: CookieService;
let childSigninRedirectSpy: any;
let childCheckinRedirectSpy: any;
let adminToolsRedirectSpy: any;
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

describe('HomeComponent', () => {

  afterEach(() => {
    cookieService.removeAll();
  });

  describe('where there is a valid machine configuration cookie present', () => {

    beforeEach(() => {
      machineIdConfig = 'valid-cookie';
      thisMachineConfig = Observable.of(MachineConfiguration.fromJson({'MachineId': '123'}));
      cookieService = new CookieService(new CookieOptions());
      cookieService.putObject('machine_config_id', '1991653a-ddb8-47fd-9d3a-86761506fa4f');
      cookieService.putObject('machine_config_details', { 'KioskConfigId': 1, 'KioskIdentifier': '1991653a-ddb8-47fd-9d3a-86761506fa4f', 'KioskName': 'Test Kiosk 1 Name', 'KioskDescription': 'Test Kiosk 1 Desc', 'KioskTypeId': 1, 'LocationId': 3, 'CongregationId': 1, 'RoomId': 1984, 'StartDate': '2016-10-27T00:00:00', 'EndDate': null });
      fixture = new HomeComponent(router, cookieService, setupServiceStub);
      childSigninRedirectSpy = spyOn(fixture, 'goToChildSignin');
      childCheckinRedirectSpy = spyOn(fixture, 'goToChildCheckin');
      adminToolsRedirectSpy = spyOn(fixture, 'goToAdminTools');
      setupErrorRedirectSpy = spyOn(fixture, 'goToSetupError');
    });

    it('should redirect to child sign in when cookie is type = Sign In', () => {
      thisMachineConfig = Observable.of(MachineConfiguration.fromJson({'MachineId': '123', 'KioskTypeId': 1}));
      fixture.ngOnInit();
      expect(childSigninRedirectSpy).toHaveBeenCalled();
    });

    it('should redirect to child checkin when cookie is type = Check In', () => {
      thisMachineConfig = Observable.of(MachineConfiguration.fromJson({'MachineId': '123', 'KioskTypeId': 2}));
      fixture.ngOnInit();
      expect(childCheckinRedirectSpy).toHaveBeenCalled();
    });

    it('should redirect to admin signin when cookie is type = Admin', () => {
      thisMachineConfig = Observable.of(MachineConfiguration.fromJson({'MachineId': '123', 'KioskTypeId': 3}));
      fixture.ngOnInit();
      expect(adminToolsRedirectSpy).toHaveBeenCalled();
    });

    it('should redirect to error page when cookie is an unknown type', () => {
      thisMachineConfig = Observable.of(MachineConfiguration.fromJson({'MachineId': '123', 'KioskTypeId': 46}));
      fixture.ngOnInit();
      expect(setupErrorRedirectSpy).toHaveBeenCalled();
    });
  });

  describe('where there is not an invalid machine configuration cookie present', () => {

    beforeEach(() => {
      machineIdConfig = undefined;
      thisMachineConfig = undefined;
      cookieService = new CookieService(new CookieOptions());
      fixture = new HomeComponent(router, cookieService, setupServiceStub);
      childSigninRedirectSpy = spyOn(fixture, 'goToChildSignin');
    });

    it('should redirect to child sign in when cookie is type = Sign In', () => {
      fixture.ngOnInit();
      expect(childSigninRedirectSpy).not.toHaveBeenCalled();
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
