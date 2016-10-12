import { Component } from '@angular/core';

import { AdminService } from '../admin.service';

@Component({
  selector: 'events',
  templateUrl: 'events.component.html',
  providers: [ AdminService ]
})
export class EventsComponent {
  events: any[];

  constructor(private adminService: AdminService) { }

  private getData(): void {
    this.adminService.getEvents().subscribe(
      events => {this.events = events},
      error => console.error(error)
    );
  }
  ngOnInit(): void {
    this.getData();
  }

}
