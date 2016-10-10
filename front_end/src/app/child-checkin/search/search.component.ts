import { Component } from '@angular/core';

@Component({
  selector: 'search',
  templateUrl: 'search.component.html',
  styleUrls: ['../scss/_number-pad.scss', ]
})
export class SearchComponent {
  private phoneNumber: string = '';

  constructor() {}

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
}
