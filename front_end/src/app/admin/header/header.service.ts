import { Injectable } from '@angular/core';
import { Subject }    from 'rxjs/Subject';
import { Event }    from '../events/event';

@Injectable()
export class HeaderService {

  // Observable sources
  private missionAnnouncedSource = new Subject<string>();
  private eventSource = new Subject<any>();

  // Observable streams
  missionAnnounced$ = this.missionAnnouncedSource.asObservable();
  eventAnnounced$ = this.eventSource.asObservable();

  // Service message commands
  announceMission(mission: string) {
    this.missionAnnouncedSource.next(mission);
  }

  announceEvent(event: any) {
    console.log("announced event", event)
    this.eventSource.next(event);
  }
}
