import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { NewFamily, NewParent, NewChild, Group } from '../../../shared/models';
import { AdminService } from '../../admin.service';
import { ApiService } from '../../../shared/services';
import { HeaderService } from '../../header/header.service';

import * as moment from 'moment';

@Component({
  selector: 'new-family-registration',
  templateUrl: 'new-family-registration.component.html'
})
export class NewFamilyRegistrationComponent implements OnInit {
  private eventId: string;
  private family: NewFamily;
  private gradeGroups: Array<Group> = [];

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private adminService: AdminService,
    private router: Router) {}

  ngOnInit() {
   this.eventId = this.route.snapshot.params['eventId'];
   this.family = new NewFamily();
   this.family.parent = new NewParent();
   this.family.children = [];

    this.apiService.getEvent(this.eventId).subscribe((event) => {
        this.family.event = event;
        this.family.children = [this.newChild()];
        this.headerService.announceEvent(event);
      },
      error => console.error(error)
    );

    this.adminService.getGradeGroups().subscribe((groups) => this.gradeGroups = groups);
  }

  updateNumberOfKids(numberOfKids: number): void {
    let tmpChildren: Array<NewChild> = [];

    for (let i = 0; i < numberOfKids; i++) {
      if (this.family.children[i] === undefined) {
        tmpChildren.push(this.newChild());
      } else {
        tmpChildren.push(this.family.children[i]);
      }
    }

    this.family.children = tmpChildren;
  }

  private newChild(): NewChild {
    let child = new NewChild();
    child.DateOfBirth = moment(this.family.event.EventStartDate).startOf('day').toDate();

    return child;
  }


  onSubmit() {
    debugger;
  }

  // TODO: Remove this when we're done
  // get diagnostic() { return JSON.stringify(this.model); }

}
