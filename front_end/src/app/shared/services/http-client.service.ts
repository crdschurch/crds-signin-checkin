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
    return this.http.get(url, this.getRequestOption(options));
  }

  post(url, data, options?: RequestOptions) {
    return this.http.post(url, data, this.getRequestOption(options));
  }

  isLoggedIn(): boolean {
    return this.authenticationToken.length > 0;
  }

  private getRequestOption(options?: RequestOptions):  RequestOptions {
    let reqOptions = (options === undefined || options === null) ? new RequestOptions() : options;
    this.createAuthorizationHeader(reqOptions.headers);

    return reqOptions;
  }

  private createAuthorizationHeader(headers?: Headers) {
    headers = (headers === undefined || headers === null) ? new Headers() : headers;

    if (!headers.has('Authorization')) {
      headers.append('Authorization', this.authenticationToken);
    }

    if (!headers.has('Content-Type')) {
      headers.append('Content-Type', 'application/json');
    }

    if (!headers.has('Access-Control-Allow-Origin')) {
      headers.append('Access-Control-Allow-Origin', '*');
    }
  }
}
