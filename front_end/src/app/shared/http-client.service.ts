import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions} from '@angular/http';

@Injectable()
export class HttpClient {
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
    let reqOptions = options === null ? new RequestOptions() : options;
    this.createAuthorizationHeader(reqOptions.headers);

    return reqOptions;
  }

  private createAuthorizationHeader(headers: Headers) {
    if (headers.has('Authorization')) {
        headers.append('Authorization', this.authenticationToken);
    }
  }
}
