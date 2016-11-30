import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Event } from '../../../shared/models';
import { ApiService } from '../../../shared/services';
import { HeaderService } from '../../header/header.service';
import { Child } from '../../../shared/models/child';

import * as moment from 'moment';

@Component({
  templateUrl: 'new-family-registration.component.html'
})
export class NewFamilyRegistrationComponent implements OnInit {
  private numberOfPossibleKids: Array<number> = [];
  private eventId: string;
  private event: Event;
  private children: Array<Child> = [];

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private router: Router) {}

  ngOnInit() {
    this.numberOfPossibleKids = Array.from({length: 12}, (v, k) => k + 1);
    this.eventId = this.route.snapshot.params['eventId'];

    this.apiService.getEvent(this.eventId).subscribe((event) => {
        this.event = event;
        this.children = [this.newChild()];
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );
  }

  updateNumberOfKids(numberOfKids: number): void {
    let tmpChildren: Array<Child> = [];

    for (let i = 0; i < numberOfKids; i++) {
      if (this.children[i] === undefined) {
        tmpChildren.push(this.newChild());
      } else {
        tmpChildren.push(this.children[i]);
      }
    }

    this.children = tmpChildren;
  }

  needGradeLevel(child: Child): boolean {
    return moment(child.DateOfBirth).isBefore(moment(this.event.EventStartDate).startOf('day').subtract(5, 'y'));
  }

  private newChild(): Child {
    let child = new Child();
    child.DateOfBirth = moment(this.event.EventStartDate).startOf('day').toDate();

    return child;
  }
}
