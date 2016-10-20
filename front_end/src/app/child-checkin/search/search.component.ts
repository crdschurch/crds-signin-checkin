import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'search',
  template: `
          <input type="tel" class="num-pad-value col-xs-12"
            readonly placeholder="Enter your phone number to check in your kids."
            value="{{phoneNumber | phoneNumber}}">
  `
})
export class SearchComponent {
  private phoneNumber: string = '';
  private error: boolean = false;

  // constructor(private router: Router) {}
  constructor() {}

  get getPhoneNumber(): string {
    return this.phoneNumber;
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
    if (this.phoneNumber.length === 10) {
      this.error = false;
      // this.router.navigate(['/child-checkin/results']);
    } else {
      this.error = true;
    }
  }
}
