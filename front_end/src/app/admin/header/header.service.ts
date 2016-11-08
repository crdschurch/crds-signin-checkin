import { Injectable } from '@angular/core';
import { Subject }    from 'rxjs/Subject';
import { Event }    from '../../shared/models';

@Injectable()
export class HeaderService {

  // Observable sources
  private eventSource = new Subject<Event>();

  // Observable streams
  eventAnnounced$ = this.eventSource.asObservable();

  // Service message commands
  announceEvent(event: Event) {
    this.eventSource.next(event);
  }
}
