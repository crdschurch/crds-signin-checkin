import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { HttpClientService } from '../shared/services';
import { PhoneNumberPipe } from '../shared/pipes/phoneNumber.pipe';
import { EventParticipants } from '../shared/models';

@Injectable()
export class ChildSigninService {
  private url: string = '';
  private phoneNumber: string = '';
  private eventParticipantsResults: EventParticipants;

  constructor(private http: HttpClientService, private router: Router) {
    this.url = `${process.env.ECHECK_API_ENDPOINT}/signin`;
  }

  getEvent() {
    return this.eventParticipantsResults.CurrentEvent;
  }

  getChildrenByPhoneNumber(phoneNumber: string): Observable<EventParticipants> {
    let pipe = new PhoneNumberPipe();
    const url = `${this.url}/children/${pipe.transform(phoneNumber, true)}`;

   if (this.eventParticipantsResults && this.eventParticipantsResults.hasParticipants() && this.phoneNumber === phoneNumber) {
      return Observable.of(this.eventParticipantsResults);
    } else {
      this.phoneNumber = phoneNumber;
      this.eventParticipantsResults = new EventParticipants();
      return this.http.get(url)
                  .map((response) => {
                    this.eventParticipantsResults = EventParticipants.fromJson(response.json());

                    return this.eventParticipantsResults;
                  })
                  .catch(this.handleError);
    }
  }

  signInChildren(eventParticipants: EventParticipants): Observable<EventParticipants> {
    const url = `${this.url}/children`;
    return this.http.post(url, eventParticipants)
                    .map(res => {
                      this.eventParticipantsResults = EventParticipants.fromJson(res.json());
                      return this.eventParticipantsResults;
                    })
                    .catch(this.handleError);
  }

  getEventParticipantsResults(): EventParticipants {
    return this.eventParticipantsResults;
  }

  printParticipantLabels(eventParticipants: EventParticipants): Observable<EventParticipants> {
    const url = `${this.url}/participants/print`;
    return this.http.post(url, eventParticipants)
                    .map(res => {
                      this.eventParticipantsResults = EventParticipants.fromJson(res.json());
                      return this.eventParticipantsResults;
                    })
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    return Observable.throw(error.json().error || 'Server error');
  }
}
