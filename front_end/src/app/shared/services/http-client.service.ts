import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions} from '@angular/http';

@Injectable()
export class HttpClientService {
  private http: Http;
  private authenticationToken: string = '';

  constructor(http: Http) {
    this.http = http;
  }

  get(url: string, options?: RequestOptions) {
    let headers = this.getRequestOption(options)
    return this.http.get(url, headers);
  }

  post(url:string, data:any, options?: RequestOptions) {
    let headers = this.getRequestOption(options)
    return this.http.post(url, data, headers);
  }

  isLoggedIn(): boolean {
    return this.authenticationToken.length > 0;
  }

  private getRequestOption(options?: RequestOptions):  RequestOptions {
    let reqOptions = (options === undefined || options === null) ? new RequestOptions() : options;
    reqOptions.headers = this.createAuthorizationHeader();

    return reqOptions;
  }

  private createAuthorizationHeader() {
    let headers =  new Headers();

    headers.set('Authorization', this.authenticationToken);

    headers.set('Content-Type', 'application/json');

    headers.set('Access-Control-Allow-Origin', '*');

    return headers;
  }
}
