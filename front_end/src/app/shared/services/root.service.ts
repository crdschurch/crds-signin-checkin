import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
//import { Event } from '../events/event';

@Injectable()
  export class RootService {

    // Observable sources
    private eventSource = new Subject<any>();
    // Observable streams
    eventAnnounced$ = this.eventSource.asObservable();
    // Service message commands
    announceEvent(event: any) {
    this.eventSource.next(event);

  }
}
