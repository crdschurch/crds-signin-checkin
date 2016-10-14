import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import '../../rxjs-operators';
import { HttpClientService } from '../../shared/services';

@Injectable()
export class SignInService {
  private url: string = '';

  constructor(private http: HttpClientService) {
    this.url = `${process.env.ECHECK_API_ENDPOINT}/authenticate`;
  }

  logIn(username: string, password: string) {
    let body = { username: username, password: password };
    return this.http.post(this.url, JSON.stringify(body))
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    // in a real world app, we may send the error to some remote logging infrastructure
    // instead of just logging it to the console
    console.error(error);
    return Observable.throw(error.json().error || 'Server error');
  }
}
