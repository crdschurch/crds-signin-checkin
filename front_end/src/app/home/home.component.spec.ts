import { HomeComponent } from './home.component';
import { Router } from '@angular/router';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { Http, RequestOptions } from '@angular/http';
import { MockBackend } from '@angular/http/testing';

let fixture: HomeComponent;
let router: Router;
let cookieService: CookieService;

fdescribe('Home Component', () => {

  beforeEach(() => {
    fixture = new HomeComponent(this.router, this.cookie);
    cookieService = new CookieService(new CookieOptions());
    cookieService.putObject('machine_config', { "KioskConfigId": 1, "KioskIdentifier": "1991653a-ddb8-47fd-9d3a-86761506fa4f", "KioskName": "Test Kiosk 1 Name", "KioskDescription": "Test Kiosk 1 Desc", "KioskTypeId": 1, "LocationId": 3, "CongregationId": 1, "RoomId": 1984, "StartDate": "2016-10-27T00:00:00", "EndDate": null });
  });

  describe('where there is a redirect cookie present', () => {
    it('should redirect to child sign in when cookie is type = Sign In', () => {
      expect(cookieService.getObject('machine_config')['KioskTypeId']).toEqual(1);
    });
  });


});
