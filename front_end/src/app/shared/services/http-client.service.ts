import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Headers, RequestOptions, Response } from '@angular/http';

@Injectable()
export class HttpClientService {
  private http: Http;
  private authenticationToken: string = '';

  constructor(http: Http) {
    this.http = http;
  }

  get(url: string, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options);
    return this.extractAuthToken(this.http.get(url, requestOptions));
  }
  put(url: string, data: any, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options)
    return this.extractAuthToken(this.http.put(url, data, requestOptions));
  }

  post(url: string, data: any, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options);
    return this.extractAuthToken(this.http.post(url, data, requestOptions));
  }

  isLoggedIn(): boolean {
    return this.authenticationToken.length > 0;
  }

  logOut(): void {
    this.authenticationToken = undefined;
  }

  private extractAuthToken(o: Observable<Response>): Observable<Response> {
    o.subscribe((res: Response) => {
      let body = res.json();
      if (body != null && body.userToken) {
        this.authenticationToken = body.userToken;
      }
    });
    return o;
  }

  private getRequestOption(options?: RequestOptions):  RequestOptions {
    let reqOptions = options || new RequestOptions();
    reqOptions.headers = this.createAuthorizationHeader(reqOptions.headers);

    return reqOptions;
  }

  private createAuthorizationHeader(headers?: Headers) {
    let reqHeaders =  headers || new Headers();
    reqHeaders.set('Authorization', this.authenticationToken);
    reqHeaders.set('Content-Type', 'application/json');
    reqHeaders.set('Accept', 'application/json, text/plain, */*');
    reqHeaders.set('Crds-Api-Key', process.env.ECHECK_API_TOKEN);

    return reqHeaders;
  }
}
