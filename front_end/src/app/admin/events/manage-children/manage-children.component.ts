import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Event, Child } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';

import * as _ from 'lodash';

@Component({
  templateUrl: 'manage-children.component.html',
  styleUrls: ['manage-children.component.scss'],
  providers: [ ],
})
export class ManageChildrenComponent implements OnInit {
  private _children: Array<Child> = [];
  childrenByRoom: any;
  ready = false;
  searchString = '';
  eventId: any;
  event: any;
  totalChildrenInEvent: number;

  constructor(private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private rootService: RootService,
    private adminService: AdminService,
    private router: Router) {
  }

  ngOnInit() {
    this.eventId = this.route.snapshot.params['eventId'];
    this.apiService.getEvent(this.eventId).subscribe((event: Event) => {
      this.event = event;
      this.headerService.announceEvent(event);

      this.adminService.getChildrenForEvent(+this.eventId).subscribe((resp) => {
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
        parents = `${hoh.Nickname}`;
        if (hoh.MobilePhone) {
          parents = `${parents} (${hoh.MobilePhone})`;
        }
      } else {
        parents = `${parents}, ${hoh.Nickname}`;
        if (hoh.MobilePhone) {
          parents = `${parents} (${hoh.MobilePhone})`;
        }
      }
    });

    return parents;
  }

  public reprint(child: Child): void {
    this.ready = false;

    this.adminService.reprint(child.EventParticipantId).subscribe((resp) => {
      this.ready = true;
      },
      (error) => {
        switch (error.status) {
          case 412:
            this.rootService.announceEvent('echeckNewFamilyAdminSetupWrong');
            break;
          default:
            this.rootService.announceEvent('generalError');
            break;
          }
          this.ready = true;
      });
  }

  public reverseSignin(child: Child) {
    this.ready = false;

    this.adminService.reverseSignin(+this.eventId, child.AssignedRoomId, child.EventParticipantId).subscribe((resp) => {
      this.children.splice(this.children.indexOf(child), 1);
      // this needs to be here so that it runs through the children setter
      // that groups the children
      this.children = this.children;
      this.ready = true;
      this.rootService.announceEvent('reverseSigninSuccess');
    }, error => (this.handleError(error)));
  }

  onSearchType(searchString) {
    this.searchString = searchString;
  }

  onClearSearch(box) {
    this.searchString = '';
    box.value = '';
    this.executeSearch();
  }

  onSearch() {
    this.executeSearch();
  }

  private executeSearch() {
    this.ready = false;
    this.adminService.getChildrenForEvent(+this.eventId, this.searchString).subscribe((resp) => {
      this.children = resp;
      // this.childrenByRoom = _(this.children).groupBy(r => r.AssignedRoomName).value();
      // this.childrenByRoom = Object.keys(this.childrenByRoom).sort().map(k => this.childrenByRoom[k]);
      this.ready = true;
    });
  }

  set children(unsortedChildren) {
    this._children = unsortedChildren.sort(function(a, b): any { return a.KCSortOrder - b.KCSortOrder; });

    // now sort children by room and set it to this.childrenByRoom
    let groupedChildren = _(this.children).groupBy(r => r.AssignedRoomName).value();
    this.childrenByRoom = Object.keys(groupedChildren).map(k => groupedChildren[k]);
  }

  get children(){
    return this._children;
  }

  private handleError (error: any) {
    if (error.status === 409) {
      this.rootService.announceEvent('cannotReverseSignin');
    } else {
      this.rootService.announceEvent('generalError');
    }

    this.ready = true;
  }
}
