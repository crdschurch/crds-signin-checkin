import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Event } from '../../../shared/models';
import { ApiService } from '../../../shared/services';
import { HeaderService } from '../../header/header.service';

@Component({
  templateUrl: 'new-family-registration.component.html'
})
export class NewFamilyRegistrationComponent implements OnInit {
  private numberOfPossibleKids: Array<number> = [];
  private eventId: string;
  private event: Event;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private router: Router) {}

  ngOnInit() {
    this.numberOfPossibleKids = Array(12).fill(12, 1, 12);
    this.eventId = this.route.snapshot.params['eventId'];

    this.apiService.getEvent(this.eventId).subscribe((event) => {
        this.event = event;
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );
  }
}
