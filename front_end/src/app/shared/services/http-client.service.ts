import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { CookieService } from 'angular2-cookie/core';

import { User } from '../models/user';

@Injectable()
export class HttpClientService {

  constructor(private http: Http, private cookie: CookieService) {}

  get(url: string, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options);
    return this.extractAuthToken(this.http.get(url, requestOptions));
  }

  put(url: string, data: any, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options);
    return this.extractAuthToken(this.http.put(url, data, requestOptions));
  }

  post(url: string, data: any, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options);
    return this.extractAuthToken(this.http.post(url, data, requestOptions));
  }

  isLoggedIn(): boolean {
    return this.user.isLoggedIn();
  }

  logOut(): void {
    let user = this.user;
    user.logOut();

    this.user = user;
  }

  getUser(): User {
    return this.user;
  }

  private extractAuthToken(o: any) {
    let sharable = o.share();
    sharable.subscribe((res: Response) => {
      let user = this.user;
      let body = res.json();

      if (body != null && body.userToken) {
        user.token = body.userToken;
      }
      if (body != null && body.refreshToken) {
        user.refreshToken = body.refreshToken;
      }

      this.user = user;
    });
    return sharable;
  }

  private getRequestOption(options?: RequestOptions):  RequestOptions {
    let reqOptions = options || new RequestOptions();
    reqOptions.headers = this.createAuthorizationHeader(reqOptions.headers);

    return reqOptions;
  }

  private createAuthorizationHeader(headers?: Headers) {
    let reqHeaders =  headers || new Headers();
    reqHeaders.set('Authorization', this.user.token);
    reqHeaders.set('Content-Type', 'application/json');
    reqHeaders.set('Accept', 'application/json, text/plain, */*');
    reqHeaders.set('Crds-Api-Key', process.env.ECHECK_API_TOKEN);

    return reqHeaders;
  }

  private get user(): User {
    let user: User;

    if (!this.cookie.getObject('user')) {
      this.cookie.putObject('user', new User());
    }

    user = Object.create(User.prototype);
    Object.assign(user, this.cookie.getObject('user'));

    return user;
  }

  private set user(value: User) {
    this.cookie.putObject('user', value);
  }
}
