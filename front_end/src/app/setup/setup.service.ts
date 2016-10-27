import { Injectable } from '@angular/core';
import { CookieService } from 'angular2-cookie/core';
import { Observable } from 'rxjs/Observable';
import { HttpClientService } from '../shared/services';
import { MachineConfiguration } from './machine-configuration';


@Injectable()
export class SetupService {
  MACHINE_CONFIG_COOKIE_NAME = 'machine_config';

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

    // // TODO: remove this when backend working
    // const mockResponse = { Site: 4, Type: 2, Guid: guid };
    // return Observable.of(new Object()).map((mock: MachineConfiguration) => {
    //   this.setMachineConfigCookie(mockResponse);
    //   return this.getMachineConfigCookie();
    // });

  }

  setMachineConfigCookie(config: MachineConfiguration) {
    this.cookieService.putObject(this.MACHINE_CONFIG_COOKIE_NAME, config);
  }

  getMachineConfigCookie(): any {
    return this.cookieService.getObject(this.MACHINE_CONFIG_COOKIE_NAME);
  }

}
