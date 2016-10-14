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
    let requestOptions = this.getRequestOption(options);
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
    this.authenticationToken = '';
  }

  private extractAuthToken(o: Observable<Response>): Observable<Response> {
    let sharable = o.share();
    sharable.map((res: Response) => {
      let body = res.json();
      if (body != null && body.userToken) {
        this.authenticationToken = body.userToken;
      }
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
    // reqHeaders.set('Authorization', this.authenticationToken);
    reqHeaders.set('Authorization', "AAEAAH_6mX6GmvyxioQBWpwnB9xgyfAZLkRBO-QGH8Bpb4gff-ooSKhPkw5cpaTR5xsvJ_rFD4wzg7s9gBUnvBtCT_PKApmRBfuOhM6_vBRFId4KR69mNEtNum6f_zYAAkAUWLC9rGaqfUx6o1BVIZrY7E8s3tokhzZJCOAt6kekssVXSavsMlB_o8rlqt4VWuPLjMWZUtYlJ3dLfyWeRQhvCeAMdltVee_5hDQxh5bFYGfxaL_78RnV1abI-lbIpSoSIlOH1i-Z31ABzxQC25SIUzDsJsogpUoaY4WH10WhhLN_tGWs1g49CyIbc66rv7qmiAbSRdgd8wANtTXZncIF2dLIAAAATGlmZXRpbWU9MTgwMCZDbGllbnRJZGVudGlmaWVyPWNsaWVudCZVc2VyPTQ4NjFhOTBjLWNhZjgtNGNjMy04ZGUxLWEyZjUwMzM1NDA3MyZTY29wZT1odHRwJTNBJTJGJTJGd3d3LnRoaW5rbWluaXN0cnkuY29tJTJGZGF0YXBsYXRmb3JtJTJGc2NvcGVzJTJGYWxsJnRzPTE0NzY0NjY2MjYmdD1Eb3ROZXRPcGVuQXV0aC5PQXV0aDIuQWNjZXNzVG9rZW4");
    reqHeaders.set('Content-Type', 'application/json');
    reqHeaders.set('Accept', 'application/json, text/plain, */*');
    reqHeaders.set('Crds-Api-Key', process.env.ECHECK_API_TOKEN);

    return reqHeaders;
  }
}
