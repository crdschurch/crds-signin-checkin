import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { RequestOptions, URLSearchParams } from '@angular/http';
import '../rxjs-operators';

import { HttpClientService } from '../shared/services';
import { PhoneNumberPipe } from '../shared/pipes/phoneNumber.pipe';
import { Child } from '../shared/models/child';

@Injectable()
export class ChildSigninService {
  private url: string = '';
  private phoneNumber: string = '';
  private childrenAvailable: Array<Child> = [];

  constructor(private http: HttpClientService) {
    this.url = `${process.env.ECHECK_API_ENDPOINT}/signin`;
  }

  getChildrenByPhoneNumber(phoneNumber: string) {
    let pipe = new PhoneNumberPipe();
    const url = `${this.url}/children/${pipe.transform(phoneNumber, true)}`;

   if (this.childrenAvailable.length > 0 && this.phoneNumber === phoneNumber) {
      return Observable.of(this.childrenAvailable);
    } else {
      this.phoneNumber = phoneNumber;
      this.childrenAvailable = [];
      return this.http.get(url)
                  .map((response) => {
                    for (let kid of response.json()) {
                      let child = Object.create(Child.prototype);
                      Object.assign(child, kid);
                      child.signIn = true;
                      this.childrenAvailable.push(child);
                    }

                    return this.childrenAvailable;
                  })
                  .catch(this.handleError);
    }
  }

  private handleError (error: any) {
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
