import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Event, Child } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';

@Component({
  templateUrl: 'manage-children.component.html',
  styleUrls: ['manage-children.component.scss'],
  providers: [ ],
})
export class ManageChildrenComponent implements OnInit {
  children: Array<Child> = [];
  ready: boolean = false;

  constructor(private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private rootService: RootService,
    private adminService: AdminService,
    private router: Router) {
  }

  ngOnInit(): void {
    let eventId = this.route.snapshot.params['eventId'];
    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.headerService.announceEvent(event);

      this.adminService.getChildrenForEvent(eventId).subscribe((resp) => {
        this.children = resp;
        this.ready = true;
      });
    });
  }

  public isReady(): boolean {
    return this.ready;
  }

  public parentAndPhoneNumbers(child: Child): string {
    let parents = '';

    child.HeadsOfHousehold.forEach(hoh => {
      if (parents === '') {
        parents = `${hoh.Nickname} (${hoh.MobilePhone})`;
      } else {
        parents = `${parents}, ${hoh.Nickname} (${hoh.MobilePhone})`;
      }
    });

    return parents;
  }

  public reverseSignin(child: Child) {
    this.adminService.reverseSignin(child.EventParticipantId);
    this.children.splice(this.children.indexOf(child), 1);
  }
}
