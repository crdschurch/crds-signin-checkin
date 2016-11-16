import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { HttpClientService } from '../shared/services';
import { PhoneNumberPipe } from '../shared/pipes/phoneNumber.pipe';
import { Child, Event, EventParticipants } from '../shared/models';

@Injectable()
export class ChildSigninService {
  private url: string = '';
  private phoneNumber: string = '';
  private childrenAvailable: Array<Child> = [];
  private eventParticipantsResults: any; // TODO should be EventParticipants
  private event: Event;

  constructor(private http: HttpClientService, private router: Router) {
    this.url = `${process.env.ECHECK_API_ENDPOINT}/signin`;
  }

  getEvent() {
    return this.event;
  }

  getChildrenByPhoneNumber(phoneNumber: string) {
    let pipe = new PhoneNumberPipe();
    const url = `${this.url}/children/${pipe.transform(phoneNumber, true)}`;

   if (this.childrenAvailable.length > 0 && this.phoneNumber === phoneNumber) {
      return Observable.of(this.childrenAvailable);
    } else {
      this.phoneNumber = phoneNumber;
      this.childrenAvailable = [];
      return this.http.get(url)
                  .map((response) => {
                    this.event = response.json().CurrentEvent;
                    for (let kid of response.json().Participants) {
                      let child = Object.create(Child.prototype);
                      Object.assign(child, kid);
                      // set all selected to true
                      // TODO: backend should probably do this
                      child.Selected = true;
                      this.childrenAvailable.push(child);
                    }

                    return this.childrenAvailable;
                  })
                  .catch(this.handleError);
    }
  }

  signInChildren(eventParticipants: EventParticipants): Observable<EventParticipants> {
    // TODO uncomment when backend complete
    // TODO EventParticipants.fromJson...
    // const url = `${this.url}/children`;
    // return this.http.post(url, eventParticipants)
    //                 .map(res => EventParticipants.fromJson(res.json()))
    //                 .catch(this.handleError);
    let eventParticipants2 = {
      CurrentEvent: { EventTitle: 'Current Event 123', EventId: 21312, EventStartDate: '123', EventType: '312', EventSite: '3', IsCurrentEvent: true, EventSiteId: 3 },
      Participants: [
        { ParticipantId: 123, ContactId: 123, HouseholdId: 123, HouseholdPositionId: 123, FirstName: 'Bob', LastName: 'string', DateOfBirth: new Date(), Selected: true, ParticipationStatusId: 123 },
        { ParticipantId: 123, ContactId: 123, HouseholdId: 123, HouseholdPositionId: 123, FirstName: 'Kenny', LastName: 'string', DateOfBirth: new Date(), Selected: true, ParticipationStatusId: 123 }
      ]
    };
    this.eventParticipantsResults = eventParticipants2;
    debugger
    return Observable.of(eventParticipants2);
  }

  // TODO should be Array<EventParticipants> {
  getEventParticipantsResults(): Array<any> {
    return this.eventParticipantsResults;
  }

  private handleError (error: any) {
    return Observable.throw(error.json().error || 'Server error');
  }
}
