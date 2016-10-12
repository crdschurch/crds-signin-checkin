import { Component } from '@angular/core';

import { EventsService } from './events.service';

@Component({
  selector: 'events',
  templateUrl: 'events.component.html',
  providers: [ EventsService ]
})
export class EventsComponent {
  events: any[];

  constructor(private eventsService: EventsService) { }

  getEvents(): void {
    console.log("getEvents")
    this.eventsService.getAll().subscribe(
      events => this.events = events,
      error => console.error(error)
    );
  }
  ngOnInit(): void {
    console.log("init")
    this.getEvents();
  }

}
