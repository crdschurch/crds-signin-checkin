import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClientService } from '../shared/services';
import { MachineConfiguration } from './machine-configuration';
import { Response, ResponseOptions } from '@angular/http';


@Injectable()
export class SetupService {

  constructor(private http: HttpClientService) {
  }

  getMachineConfiguration(guid: string) {
    // const url = `${process.env.ECHECK_API_ENDPOINT}/setup/${guid}`;
    // TODO: uncomment when backend working
    // return this.http.get(url)
    //                 .map(res => MachineConfiguration.fromJson(res.json()))
    //                 .catch(this.handleError);

    // TODO: remove this when backend working
    const mockResponse = { Site: 4, Type: 2, Guid: guid };
    return Observable.of(new Object()).map(mock => mockResponse);

  }
}
