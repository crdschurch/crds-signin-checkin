import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { HttpClientService } from '../../shared/services';
import { Router } from '@angular/router';

import { Event } from '../models/event';

@Component({
  selector: 'events',
  templateUrl: 'events.component.html',
  providers: [ AdminService, HttpClientService ]
})
export class EventsComponent implements OnInit {
  events: Event[];

  constructor(private adminService: AdminService, private httpClientService: HttpClientService, private router: Router) { }

  private getData(): void {
    this.adminService.getEvents().subscribe(
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
