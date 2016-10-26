import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ChildSigninService } from '../child-signin.service';

@Component({
  selector: 'search',
  templateUrl: 'search.component.html'
})
export class SearchComponent {
  phoneNumber: string = '';
  error: boolean = false;

  constructor(private router: Router, private childSigninService: ChildSigninService) {}

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
      this.error = false;
      this.childSigninService.getChildrenByPhoneNumber(this.phoneNumber).subscribe(() => {
        this.router.navigate(['/child-signin/available-children', this.phoneNumber]);
      });
    } else {
      this.error = true;
    }
  }
}
