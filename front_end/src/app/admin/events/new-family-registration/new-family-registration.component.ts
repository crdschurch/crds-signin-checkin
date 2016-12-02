import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { NewFamily, NewParent, NewChild, Group } from '../../../shared/models';
import { AdminService } from '../../admin.service';
import { ApiService, RootService } from '../../../shared/services';
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
  private processing: boolean;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private adminService: AdminService,
    private rootService: RootService,
    private router: Router) {}

  ngOnInit() {
    this.setUp();
  }

  setUp() {
   this.processing = false;
   this.eventId = this.route.snapshot.params['eventId'];
   this.family = new NewFamily();
   this.family.parent = new NewParent();
   this.family.children = [];

    this.apiService.getEvent(this.eventId).subscribe((event) => {
        this.family.event = event;
        this.family.children = [this.newChild()];
        this.headerService.announceEvent(event);
        this.adminService.getGradeGroups().subscribe((groups) => {
            this.gradeGroups = groups;
          },
          error => console.error(error)
        );
      },
      error => console.error(error)
    );
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

  onSubmit() {
    this.processing = true;
    this.adminService.createNewFamily(this.family).subscribe((res) => {
      this.setUp();
    }, (error) => {
      this.rootService.announceEvent('generalError');
      this.processing = false;
    });
  }

  private newChild(): NewChild {
    let child = new NewChild();
    child.DateOfBirth = moment(this.family.event.EventStartDate).startOf('day').toDate();

    return child;
  }
}
