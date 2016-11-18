import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { CookieService } from 'angular2-cookie/core';
import { User, MachineConfiguration } from '../models';
import { Observable } from 'rxjs/Observable';
import { UserService } from './user.service';

@Injectable()
export class HttpClientService {

  constructor(private http: Http, private cookie: CookieService, private userService: UserService) {}

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

  private getUser(): User {
    return this.userService.getUser();
  }

  private extractAuthToken(o: any) {
    let sharable = o.share();
    sharable.subscribe(
      (res: Response) => {
        let user = this.getUser();
        if (res.headers.get('Authorization')) {
          user.token = res.headers.get('Authorization');
        };
        if (res.headers.get('RefreshToken')) {
          user.refreshToken = res.headers.get('RefreshToken');
        }
        this.userService.setUser(user);
      },
      (error) => {
        return Observable.throw(error || 'Server error');
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
    let user = this.getUser();
    reqHeaders.set('Authorization', user.token);
    reqHeaders.set('RefreshToken', user.refreshToken);
    reqHeaders.set('Content-Type', 'application/json');
    reqHeaders.set('Accept', 'application/json, text/plain, */*');
    reqHeaders.set('Crds-Api-Key', process.env.ECHECK_API_TOKEN);

    const machineConfig = MachineConfiguration.fromJson( this.cookie.getObject(MachineConfiguration.COOKIE_NAME_DETAILS) );
    if (machineConfig) {
      if (machineConfig.CongregationId) {
        reqHeaders.set('Crds-Site-Id', machineConfig.CongregationId.toString());
      }
      if (machineConfig.KioskConfigId) {
        reqHeaders.set('Crds-Kiosk-Identifier', machineConfig.KioskIdentifier);
      }
    }

    return reqHeaders;
  }
}
