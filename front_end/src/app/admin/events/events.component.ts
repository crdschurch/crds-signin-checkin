import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { HttpClientService } from '../../shared/services';
import { Router } from '@angular/router';

import { Event } from '../models/event';

import * as moment from 'moment';

@Component({
  selector: 'events',
  templateUrl: 'events.component.html',
  providers: [ AdminService, HttpClientService ]
})
export class EventsComponent implements OnInit {
  events: Event[];

  constructor(private adminService: AdminService, private httpClientService: HttpClientService, private router: Router) { }

  private getData(): void {
    // TODO these should come from dropdown
    let startDate = moment().format("YYYY-MM-DD");
    let endDate = moment().add(7, "days").format("YYYY-MM-DD");

    this.adminService.getEvents(startDate, endDate, 34532324).subscribe(
      events => {this.events = events;},
      error => console.error(error)
    );
  }
  ngOnInit(): void {
    this.getData();
  }

  logout(): void {
    this.httpClientService.logOut();
    this.router.navigate(['/admin/sign-in']);
  }

}
