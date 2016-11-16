import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ChildSigninService } from '../child-signin.service';
import { RootService } from '../../shared/services';

@Component({
  selector: 'search',
  templateUrl: 'search.component.html'
})
export class SearchComponent {
  private isReady: boolean = true;
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
    this.isReady = false;
    if (this.phoneNumber.length === 10) {
      this.childSigninService.getChildrenByPhoneNumber(this.phoneNumber).subscribe((availableChildren) => {
        this.isReady = true;
        if (availableChildren.length > 0) {
          this.router.navigate(['/child-signin/available-children', this.phoneNumber]);
        } else {
          this.rootService.announceEvent('kcChildSigninNoAvailableChildren');
        }
      }, (error) => {
        debugger
      });
    } else {
      this.isReady = true;
      this.rootService.announceEvent('kcChildSigninPhoneNumberNotValid');
    }
  }
}
