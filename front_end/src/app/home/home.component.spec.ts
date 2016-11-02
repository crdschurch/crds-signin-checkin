import { HomeComponent } from './home.component';
import { Router } from '@angular/router';
import { CookieService, CookieOptions } from 'angular2-cookie/core';

let fixture: HomeComponent;
let router: Router;
let cookieService: CookieService;
let childCheckinRedirectSpy: any;

fdescribe('Home Component', () => {

  afterEach(() => {
    cookieService.removeAll();
  });

  describe('where there is a redirect cookie present', () => {

    beforeEach(() => {
      cookieService = new CookieService(new CookieOptions());
      cookieService.putObject('machine_config_details', { 'KioskConfigId': 1, 'KioskIdentifier': '1991653a-ddb8-47fd-9d3a-86761506fa4f', 'KioskName': 'Test Kiosk 1 Name', 'KioskDescription': 'Test Kiosk 1 Desc', 'KioskTypeId': 1, 'LocationId': 3, 'CongregationId': 1, 'RoomId': 1984, 'StartDate': '2016-10-27T00:00:00', 'EndDate': null });
      fixture = new HomeComponent(router, cookieService);
      childCheckinRedirectSpy = spyOn(fixture, 'goToChildSignin');
    });

    it('should redirect to child sign in when cookie is type = Sign In', () => {
      expect(cookieService.getObject('machine_config_details')['KioskTypeId']).toEqual(1);
      fixture.ngOnInit();
      expect(childCheckinRedirectSpy).toHaveBeenCalled();
    });

  });

  describe('where there is not a redirect cookie present', () => {

    beforeEach(() => {
      cookieService = new CookieService(new CookieOptions());
      fixture = new HomeComponent(router, cookieService);
      childCheckinRedirectSpy = spyOn(fixture, 'goToChildSignin');
    });

    it('should redirect to child sign in when cookie is type = Sign In', () => {
      fixture.ngOnInit();
      expect(childCheckinRedirectSpy).not.toHaveBeenCalled();
    });

  });


});
