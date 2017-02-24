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
  searchString: '';

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

    this.adminService.reverseSignin(child.EventParticipantId).subscribe((resp) => {
      this.children.splice(this.children.indexOf(child), 1);
      this.ready = true;
      this.rootService.announceEvent('reverseSigninSuccess');
    }, error => (this.handleError(error)));
  }

  onSearchType(searchString) {
    this.searchString = searchString;
  }

  onClearSearch(box) {
    console.log('clear')
    this.searchString = '';
    box.value = '';
  }

  onSearch() {
    console.log('search', this.searchString)
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
