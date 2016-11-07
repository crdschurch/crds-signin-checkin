import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { HttpClientService } from '../shared/services';
import { Child } from '../shared/models/child';

@Injectable()
export class ChildCheckinService {
  private url: string = '';

  constructor(private http: HttpClientService) {
    this.url = `${process.env.ECHECK_API_ENDPOINT}/checkin`;
  }

  getChildrenForRoom(roomId: number, eventId: number = null) {
    const url = (eventId == null) ? `${this.url}/children/${roomId}` : `${this.url}/children/${roomId}?eventId=${eventId}`;

    return this.http.get(url).map((response) => {
        let childrenAvailable: Array<Child> = [];

        for (let kid of response.json().Participants) {
          let child = Object.create(Child.prototype);
          Object.assign(child, kid);
          // set all selected to true
          // TODO: backend should probably do this
          child.Selected = true;
          childrenAvailable.push(child);
        }

        return childrenAvailable;
      }).
      catch(this.handleError);
  }

  checkInChildren(child: Child) {
    const url = `${this.url}/event/participant`;
    child.toggleCheckIn();

    return this.http.put(url, child)
                    .map(res => Child.fromJson(res.json()))
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    return Observable.throw(error.json().error || 'Server error');
  }
}
