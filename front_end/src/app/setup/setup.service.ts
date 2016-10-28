import { Injectable } from '@angular/core';
import { CookieService } from 'angular2-cookie/core';
import { Observable } from 'rxjs/Observable';
import { HttpClientService } from '../shared/services';
import { MachineConfiguration } from './machine-configuration';


@Injectable()
export class SetupService {
  constructor(private http: HttpClientService, private cookieService: CookieService) {
  }

  getMachineConfiguration(guid: string) {
    // TODO: uncomment when backend working
    const url = `${process.env.ECHECK_API_ENDPOINT}/kiosks/${guid}`;
    return this.http.get(url)
                    .map(res => {
                        this.setMachineConfigCookie(res.json());
                        console.log(this.getMachineConfigCookie())
                        return this.getMachineConfigCookie();
                    })
                    .catch(err => console.error(err));
  }

  setMachineConfigCookie(config: MachineConfiguration) {
    this.cookieService.putObject(MachineConfiguration.COOKIE_NAME, config);
  }

  getMachineConfigCookie(): any {
    return this.cookieService.getObject(MachineConfiguration.COOKIE_NAME);
  }

}
