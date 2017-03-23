import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ChildSigninService } from '../child-signin.service';
import { RootService } from '../../shared/services';
import { EventParticipants } from '../../shared/models';

@Component({
  selector: 'search',
  templateUrl: 'search.component.html'
})
export class SearchComponent implements OnInit {
  private isReady = true;
  phoneNumber = '';

  constructor(private router: Router,
              private childSigninService: ChildSigninService,
              private rootService: RootService) {}

  ngOnInit() {
    this.childSigninService.reset();
  }

  setPhoneNumber(num: string) {
    if (this.phoneNumber.length < 10) {
      this.phoneNumber = `${this.phoneNumber}${num}`;
    }
  }

  delete(): void {
    this.phoneNumber = this.phoneNumber.slice(0, this.phoneNumber.length - 1);
  }

  clear(): void {
    this.phoneNumber = '';
  }

  next(): void {
    this.isReady = false;
    if (this.phoneNumber.length === 10) {
      this.childSigninService.getChildrenByPhoneNumber(this.phoneNumber).subscribe(
        (result: EventParticipants) => {
        this.isReady = true;
        if (result.hasParticipants()) {
          this.router.navigate(['/child-signin/available-children', this.phoneNumber]);
        } else {
          this.rootService.announceEvent('kcChildSigninNoAvailableChildren');
        }
      }, (err) => {
        this.isReady = true;
        if (err === 'No current events for site') {
          this.rootService.announceEvent('noCurrentEvent');
        } else {
          this.rootService.announceEvent('generalError');
        }
      }
    );
    } else {
      this.isReady = true;
      this.rootService.announceEvent('kcChildSigninPhoneNumberNotValid');
    }
  }
}
