import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ChildSigninService } from '../child-signin.service';
import { RootService } from '../../shared/services';

@Component({
  selector: 'search',
  templateUrl: 'search.component.html'
})
export class SearchComponent {
  phoneNumber: string = '';

  constructor(private router: Router,
              private childSigninService: ChildSigninService,
              private rootService: RootService) {}

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
    if (this.phoneNumber.length === 10) {
      this.childSigninService.getChildrenByPhoneNumber(this.phoneNumber).subscribe((availableChildren) => {
        if (availableChildren.length > 0) {
          this.router.navigate(['/child-signin/available-children', this.phoneNumber]);
        } else {
          this.rootService.announceEvent('kcChildSigninNoAvailableChildren');
        }
      });
    } else {
      this.rootService.announceEvent('kcChildSigninPhoneNumberNotValid');
    }
  }
}
