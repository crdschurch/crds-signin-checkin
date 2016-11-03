import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { CookieService } from 'angular2-cookie/core';
import { HttpClientService } from '../shared/services';
import { MachineConfiguration } from './machine-configuration';


@Injectable()
export class SetupService {
  constructor(private http: HttpClientService, private cookieService: CookieService) {
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
    this.cookieService.putObject(MachineConfiguration.COOKIE_NAME_ID, guid);
  }

  getMachineIdConfigCookie(): any {
    return this.cookieService.getObject(MachineConfiguration.COOKIE_NAME_ID);
  }

  setMachineDetailsConfigCookie(config: MachineConfiguration) {
    this.cookieService.putObject(MachineConfiguration.COOKIE_NAME_DETAILS, config);
  }

  getMachineDetailsConfigCookie(): any {
    return this.cookieService.getObject(MachineConfiguration.COOKIE_NAME_DETAILS);
  }

}