import { Component, OnInit } from '@angular/core';
// import {Observable} from 'rxjs/Rx';

import { Child } from '../../shared/models/child';
import { Event } from '../../shared/models/event';
import { ChildCheckinService } from '../child-checkin.service';
import { RootService } from '../../shared/services';
import { SetupService } from '../../shared/services';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss' ]
})

export class RoomComponent implements OnInit {

  private _children: Array<Child> = [];
  private update: boolean = true;
  subscription: Subscription;

  constructor(private childCheckinService: ChildCheckinService, private rootService: RootService, private setupService: SetupService) {
    // subscribe to forceChildReload so that the parent (ChildCheckinComponent)
    // can talk to the child (RoomComponent) and tell it when to reload children
    this.subscription = childCheckinService.forceChildReload$.subscribe(
      astronaut => {
        this.setup(this);
    });
  }

  ngOnInit() {
    this.childCheckinService.roomComp = this;
    this.childCheckinService.roomSetUpFunc = this.setup;
    this.setup(this);
  }

  setup(comp) {
    if (comp) {
      let roomId: number = comp.setupService.getMachineDetailsConfigCookie().RoomId;
      let event: Event = comp.childCheckinService.selectedEvent;
      if (roomId && event) {
        comp.childCheckinService.getChildrenForRoom(roomId, event.EventId).subscribe((children) => {
          comp.children = children;
        }, (error) => {
          console.error(error);
          comp.rootService.announceEvent('generalError');
        });

        // This is temp. until we add websockets to do an actual update
        // We will update the rooms information every 15 seconds
        // Observable.interval(15000)
        //   .mergeMap(() => comp.childCheckinService.getChildrenForRoom(roomId, event.EventId))
        //   .subscribe((children: Child[]) => {
        //       if (comp.update) {
        //         comp.children = children;
        //       }
        //     },
        //     (error: any) => console.error(error)
        //   );
      }
    }
  }

  checkedIn(): Array<Child> {
    return this.children.filter( (obj: Child) => { return obj.checkedIn(); } );
  }

  signedIn(): Array<Child> {
    return this.children.filter( (obj: Child) => { return !obj.checkedIn(); } );
  }

  toggleCheckIn(child: Child) {
    this.update = false;
    this.childCheckinService.checkInChildren(child).subscribe(() => {
      this.update = true;
    }, (error) => {
      console.error(error);
      this.rootService.announceEvent('generalError');
    });
  }

  get children(): Array<Child> {
    return this._children;
  }

  set children(child: Array<Child>) {
    this._children = child;
  }
}
