import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Event } from '../event';
import { HeaderService } from '../../header/header.service';
import { AdminService } from '../../admin.service';

@Component({
  selector: '.event-reset',
  templateUrl: 'event-reset.component.html',
  styleUrls: ['event-reset.component.scss'],
  providers: [ ],
})
export class EventResetComponent implements OnInit {
  targetEvent: Event;
  events: Event[];
  sourceEventDate: Date;

  constructor(private route: ActivatedRoute,
    private adminService: AdminService,
    private headerService: HeaderService) {
  }

  ngOnInit(): void {
    let eventId = this.route.snapshot.params['eventId'];

    this.adminService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.headerService.announceEvent(event);
    });

  }
}
