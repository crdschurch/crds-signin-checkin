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
    let requestOptions = this.getRequestOption(options)
    return this.http.get(url, requestOptions);
  }

  put(url: string, data: any, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options)
    return this.http.put(url, data, requestOptions);
  }

  post(url: string, data: any, options?: RequestOptions) {
    let requestOptions = this.getRequestOption(options)
    return this.http.post(url, data, requestOptions);
  }

  isLoggedIn(): boolean {
    return this.authenticationToken.length > 0;
  }

  logOut(): void {
    this.authenticationToken = undefined;
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
    reqHeaders.set('Crds-Api-Key', process.env.ECHECK_API_TOKEN);

    return reqHeaders;
  }
}
