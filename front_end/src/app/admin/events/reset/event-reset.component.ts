import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Event } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { ApiService } from '../../../shared/services';

@Component({
  selector: '.event-reset',
  templateUrl: 'event-reset.component.html',
  styleUrls: ['event-reset.component.scss'],
  providers: [ ],
})
export class EventResetComponent implements OnInit {
  targetEvent: Event;

  constructor(private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService) {
  }

  ngOnInit(): void {
    let eventId = this.route.snapshot.params['eventId'];

    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.headerService.announceEvent(event);
    });
  }
}
