import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { HttpClientService } from '.';
import { MachineConfiguration } from '../models';


@Injectable()
export class SetupService {
  private persistentCookieOptions: CookieOptions;

  constructor(private http: HttpClientService, private cookieService: CookieService) {
    this.persistentCookieOptions = new CookieOptions();
    // Maximum expiration date for a cookie, to avoid the 2038 bug, is 2038-01-19 04:14:07
    this.persistentCookieOptions.expires = new Date(2038, 0, 19, 4, 14, 7);
  }

  getThisMachineConfiguration() {
    let guid = this.getMachineIdConfigCookie();
    const url = `${process.env.ECHECK_API_ENDPOINT}/kiosks/${guid}`;
    return this.http.get(url)
                    .map(res => {
                      this.setMachineDetailsConfigCookie(res.json());
                      return this.getMachineDetailsConfigCookie();
                    })
                    .catch(error => {
                      return Observable.throw('Server error');
                    });
  }

  getNewMachineConfiguration(guid: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/kiosks/${guid}`;
    this.setMachineIdConfigCookie(guid);
    return this.http.get(url)
                    .map(res => {
                        this.setMachineDetailsConfigCookie(res.json());
                        return this.getMachineDetailsConfigCookie();
                    })
                    .catch(err => console.error(err));
  }

  setMachineIdConfigCookie(guid: string) {
    this.cookieService.putObject(MachineConfiguration.COOKIE_NAME_ID, guid, this.persistentCookieOptions);
  }

  getMachineIdConfigCookie(): any {
    return this.cookieService.getObject(MachineConfiguration.COOKIE_NAME_ID);
  }

  setMachineDetailsConfigCookie(config: MachineConfiguration) {
    this.cookieService.putObject(MachineConfiguration.COOKIE_NAME_DETAILS, config);
  }

  getMachineDetailsConfigCookie(): MachineConfiguration {
    return MachineConfiguration.fromJson(this.cookieService.getObject(MachineConfiguration.COOKIE_NAME_DETAILS));
  }
}
