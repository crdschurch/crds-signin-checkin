import { Component, OnInit } from '@angular/core';
// import {Observable} from 'rxjs/Rx';

import { Child } from '../../shared/models/child';
import { Event } from '../../shared/models/event';
import { ChildCheckinService } from '../child-checkin.service';
import { RootService } from '../../shared/services';
import { SetupService } from '../../shared/services';
import { Subscription } from 'rxjs/Subscription';
import { ChannelEvent, ChannelService } from '../../shared/services';
import { Constants } from '../../shared/constants';

import * as _ from 'lodash';

@Component({
  selector: 'room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss' ]
})

export class RoomComponent implements OnInit {

  private _children: Array<Child> = [];
  private update = true;
  private event: Event;
  private roomId: number;
  subscription: Subscription;

  constructor(private childCheckinService: ChildCheckinService, private rootService: RootService,
    private setupService: SetupService, private channelService: ChannelService) {
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
      comp.roomId = comp.setupService.getMachineDetailsConfigCookie().RoomId;
      comp.event = comp.childCheckinService.selectedEvent;
      if (comp.roomId && comp.event) {
        comp.childCheckinService.getChildrenForRoom(comp.roomId, comp.event.EventId).subscribe((children) => {
          comp.children = children;
        }, (error) => {
          comp.rootService.announceEvent('generalError');
        });

        // Get an observable for events emitted on this channel
        comp.channelService.unsubAll(Constants.CheckinParticipantsChannel);
        let channelName = `${Constants.CheckinParticipantsChannel}${comp.event.EventId}${comp.roomId}`;
        comp.channelService.sub(channelName).subscribe(
          (x: ChannelEvent) => {
            if (x.Name === 'Add') {
              for (let kid of x.Data) {
                let child = Object.create(Child.prototype);
                Object.assign(child, kid);
                // set all selected to true
                // TODO: backend should probably do this
                child.Selected = true;
                child.AssignedRoomId = comp.roomId;
                comp._children.push(child);
              }
            } else if (x.Name === 'RemoveSignIn' || x.Name === 'RemoveCheckIn') {
              let data = x.Data;
              if (data.OriginalRoomId !== data.OverRideRoomId) {
                comp.children = comp.children.filter( (obj: Child) => { return obj.EventParticipantId !== data.EventParticipantId; } );
              }
            }
          },
          (error: any) => {
            console.warn('Attempt to join channel failed!', error);
          }
        );
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
    this.childCheckinService.checkInChildren(child, this.event.EventId).subscribe(() => {
      this.update = true;
    }, (error) => {
      this.rootService.announceEvent('generalError');
    });
  }

  get children(): Array<Child> {
    return _.sortBy(this._children, ['Nickname', 'LastName', 'CallNumber']);
  }

  set children(children: Array<Child>) {
    this._children = children;
  }
}
