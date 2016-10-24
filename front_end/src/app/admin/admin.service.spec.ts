/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { AdminService } from './admin.service';
import { HttpClientService } from '../shared/services/http-client.service';
import { Http, RequestOptions, Headers, Response, ResponseOptions } from '@angular/http';
import { MockConnection, MockBackend } from '@angular/http/testing';
import { CookieService, CookieOptions } from 'angular2-cookie/core';

fdescribe('AdminService', () => {
  let fixture: AdminService;
  let httpClientService: HttpClientService;
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
    httpClientService = new HttpClientService(http, cookie);
    fixture = new AdminService(httpClientService);
  });

  // it('should successfully get list of Events', () => {});
  // it('should successfully get list of Rooms', () => {});
  // it('should successfully update a Room', () => {});
  // it('should successfully get RoomGroups', () => {});
  // it('should successfully update a RoomGroup', () => {});
  it('should successfully get list of RoomGroups', () => {
    responseObject = [{'Id':9014,'Name':'Nursery','Ranges':[{'Id':9020,'Name':'0-1','Selected':true,'SortOrder':0,'TypeId':102},{'Id':9021,'Name':'1-2','Selected':false,'SortOrder':1,'TypeId':102},{'Id':9022,'Name':'2-3','Selected':false,'SortOrder':2,'TypeId':102},{'Id':9023,'Name':'3-4','Selected':false,'SortOrder':3,'TypeId':102},{'Id':9024,'Name':'4-5','Selected':false,'SortOrder':4,'TypeId':102},{'Id':9025,'Name':'5-6','Selected':false,'SortOrder':5,'TypeId':102},{'Id':9026,'Name':'6-7','Selected':false,'SortOrder':6,'TypeId':102},{'Id':9027,'Name':'7-8','Selected':false,'SortOrder':7,'TypeId':102},{'Id':9028,'Name':'8-9','Selected':false,'SortOrder':8,'TypeId':102},{'Id':9029,'Name':'9-10','Selected':false,'SortOrder':9,'TypeId':102},{'Id':9030,'Name':'10-11','Selected':false,'SortOrder':10,'TypeId':102},{'Id':9031,'Name':'11-12','Selected':false,'SortOrder':11,'TypeId':102}],'SortOrder':0,'TypeId':102,'Selected':false},{'Id':9015,'Name':'First Year','Ranges':[{'Id':9002,'Name':'Jan','Selected':false,'SortOrder':0,'TypeId':102},{'Id':9003,'Name':'Feb','Selected':false,'SortOrder':1,'TypeId':102},{'Id':9004,'Name':'Mar','Selected':false,'SortOrder':2,'TypeId':102},{'Id':9005,'Name':'Apr','Selected':false,'SortOrder':3,'TypeId':102},{'Id':9006,'Name':'May','Selected':false,'SortOrder':4,'TypeId':102},{'Id':9007,'Name':'Jun','Selected':false,'SortOrder':5,'TypeId':102},{'Id':9008,'Name':'Jul','Selected':false,'SortOrder':6,'TypeId':102},{'Id':9009,'Name':'Aug','Selected':false,'SortOrder':7,'TypeId':102},{'Id':9010,'Name':'Sep','Selected':false,'SortOrder':8,'TypeId':102},{'Id':9011,'Name':'Oct','Selected':false,'SortOrder':9,'TypeId':102},{'Id':9012,'Name':'Nov','Selected':false,'SortOrder':10,'TypeId':102},{'Id':9013,'Name':'Dec','Selected':false,'SortOrder':11,'TypeId':102}],'SortOrder':1,'TypeId':102,'Selected':false},{'Id':9016,'Name':'Second Year','Ranges':[{'Id':9002,'Name':'Jan','Selected':false,'SortOrder':0,'TypeId':102},{'Id':9003,'Name':'Feb','Selected':false,'SortOrder':1,'TypeId':102},{'Id':9004,'Name':'Mar','Selected':false,'SortOrder':2,'TypeId':102},{'Id':9005,'Name':'Apr','Selected':false,'SortOrder':3,'TypeId':102},{'Id':9006,'Name':'May','Selected':false,'SortOrder':4,'TypeId':102},{'Id':9007,'Name':'Jun','Selected':false,'SortOrder':5,'TypeId':102},{'Id':9008,'Name':'Jul','Selected':false,'SortOrder':6,'TypeId':102},{'Id':9009,'Name':'Aug','Selected':false,'SortOrder':7,'TypeId':102},{'Id':9010,'Name':'Sep','Selected':false,'SortOrder':8,'TypeId':102},{'Id':9011,'Name':'Oct','Selected':false,'SortOrder':9,'TypeId':102},{'Id':9012,'Name':'Nov','Selected':false,'SortOrder':10,'TypeId':102},{'Id':9013,'Name':'Dec','Selected':false,'SortOrder':11,'TypeId':102}],'SortOrder':2,'TypeId':102,'Selected':false},{'Id':9017,'Name':'Third Year','Ranges':[{'Id':9002,'Name':'Jan','Selected':false,'SortOrder':0,'TypeId':102},{'Id':9003,'Name':'Feb','Selected':false,'SortOrder':1,'TypeId':102},{'Id':9004,'Name':'Mar','Selected':false,'SortOrder':2,'TypeId':102},{'Id':9005,'Name':'Apr','Selected':false,'SortOrder':3,'TypeId':102},{'Id':9006,'Name':'May','Selected':false,'SortOrder':4,'TypeId':102},{'Id':9007,'Name':'Jun','Selected':false,'SortOrder':5,'TypeId':102},{'Id':9008,'Name':'Jul','Selected':false,'SortOrder':6,'TypeId':102},{'Id':9009,'Name':'Aug','Selected':false,'SortOrder':7,'TypeId':102},{'Id':9010,'Name':'Sep','Selected':false,'SortOrder':8,'TypeId':102},{'Id':9011,'Name':'Oct','Selected':false,'SortOrder':9,'TypeId':102},{'Id':9012,'Name':'Nov','Selected':false,'SortOrder':10,'TypeId':102},{'Id':9013,'Name':'Dec','Selected':false,'SortOrder':11,'TypeId':102}],'SortOrder':3,'TypeId':102,'Selected':false},{'Id':9018,'Name':'Fourth Year','Ranges':[{'Id':9002,'Name':'Jan','Selected':false,'SortOrder':0,'TypeId':102},{'Id':9003,'Name':'Feb','Selected':false,'SortOrder':1,'TypeId':102},{'Id':9004,'Name':'Mar','Selected':false,'SortOrder':2,'TypeId':102},{'Id':9005,'Name':'Apr','Selected':false,'SortOrder':3,'TypeId':102},{'Id':9006,'Name':'May','Selected':false,'SortOrder':4,'TypeId':102},{'Id':9007,'Name':'Jun','Selected':false,'SortOrder':5,'TypeId':102},{'Id':9008,'Name':'Jul','Selected':false,'SortOrder':6,'TypeId':102},{'Id':9009,'Name':'Aug','Selected':false,'SortOrder':7,'TypeId':102},{'Id':9010,'Name':'Sep','Selected':false,'SortOrder':8,'TypeId':102},{'Id':9011,'Name':'Oct','Selected':false,'SortOrder':9,'TypeId':102},{'Id':9012,'Name':'Nov','Selected':false,'SortOrder':10,'TypeId':102},{'Id':9013,'Name':'Dec','Selected':false,'SortOrder':11,'TypeId':102}],'SortOrder':4,'TypeId':102,'Selected':false},{'Id':9019,'Name':'Fifth Year','Ranges':[{'Id':9002,'Name':'Jan','Selected':false,'SortOrder':0,'TypeId':102},{'Id':9003,'Name':'Feb','Selected':false,'SortOrder':1,'TypeId':102},{'Id':9004,'Name':'Mar','Selected':false,'SortOrder':2,'TypeId':102},{'Id':9005,'Name':'Apr','Selected':false,'SortOrder':3,'TypeId':102},{'Id':9006,'Name':'May','Selected':false,'SortOrder':4,'TypeId':102},{'Id':9007,'Name':'Jun','Selected':false,'SortOrder':5,'TypeId':102},{'Id':9008,'Name':'Jul','Selected':false,'SortOrder':6,'TypeId':102},{'Id':9009,'Name':'Aug','Selected':false,'SortOrder':7,'TypeId':102},{'Id':9010,'Name':'Sep','Selected':false,'SortOrder':8,'TypeId':102},{'Id':9011,'Name':'Oct','Selected':false,'SortOrder':9,'TypeId':102},{'Id':9012,'Name':'Nov','Selected':false,'SortOrder':10,'TypeId':102},{'Id':9013,'Name':'Dec','Selected':false,'SortOrder':11,'TypeId':102}],'SortOrder':5,'TypeId':102,'Selected':false},{'Id':9032,'Name':'Kindergarten','Ranges':null,'SortOrder':5,'TypeId':104,'Selected':false},{'Id':9033,'Name':'First Grade','Ranges':null,'SortOrder':6,'TypeId':104,'Selected':false},{'Id':9034,'Name':'Second Grade','Ranges':null,'SortOrder':7,'TypeId':104,'Selected':false},{'Id':9035,'Name':'Third Grade','Ranges':null,'SortOrder':8,'TypeId':104,'Selected':false},{'Id':9036,'Name':'Fourth Grade','Ranges':null,'SortOrder':9,'TypeId':104,'Selected':false},{'Id':9037,'Name':'Fifth Grade','Ranges':null,'SortOrder':10,'TypeId':104,'Selected':false},{'Id':9038,'Name':'Sixth Grade','Ranges':null,'SortOrder':11,'TypeId':104,'Selected':false},{'Id':9039,'Name':'CSM','Ranges':null,'SortOrder':12,'TypeId':104,'Selected':false}]
    fixture.getRoomGroups('event-id-123', 'room-id-3242').subscribe((res: Response) => {
      expect(res.json()).toEqual(responseObject);
    });

  });
});
