import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { Observable } from 'rxjs/Observable';

import { Child, Room } from '../shared/models';
import { Constants } from '../shared/constants';
import { Event, MachineConfiguration } from '../shared/models';
import { ApiService, SetupService, RootService, ChannelEvent, ChannelService } from '../shared/services';
import { ChildCheckinService } from './child-checkin.service';

@Component({
  selector: 'child-checkin',
  templateUrl: 'child-checkin.component.html',
  styleUrls: ['child-checkin.component.scss', 'scss/_stepper.scss' ],
  providers: [ ChildCheckinService ]
})
export class ChildCheckinComponent implements OnInit {
  @ViewChild('serviceSelectModal') public serviceSelectModal: ModalDirective;
  @ViewChild('childDetailModal') public childDetailModal: ModalDirective;
  @ViewChild('childSearchModal') public childSearchModal: ModalDirective;
  private kioskDetails: MachineConfiguration;

  clock = Observable.interval(10000).startWith(0).map(() => new Date());
  thisSiteName: string;
  todaysEvents: Event[];
  ready: boolean;
  isOverrideProcessing: boolean;
  callNumber = '';
  overrideChild: Child = new Child();
  room: Room = new Room();

  constructor(private setupService: SetupService,
    private apiService: ApiService,
    private childCheckinService: ChildCheckinService,
    private rootService: RootService,
    private channelService: ChannelService) {

    this.kioskDetails = new MachineConfiguration();
    this.ready = false;
    this.isOverrideProcessing = false;
  }

  public ngOnInit() {
    this.getData();
  }

  private getData() {
    let today = new Date();
    this.kioskDetails = this.setupService.getMachineDetailsConfigCookie();
    this.thisSiteName = this.getKioskDetails() ? this.getKioskDetails().CongregationName : null;
    this.apiService.getEvents(today, today).subscribe(
      events => {
        if (!events.length) {
          return this.ready = true;
        }
        this.todaysEvents = [];
        // transform to Event
        for (let event of events) {
          this.todaysEvents.push(Event.fromJson(event));
        }

        if (this.todaysEvents && this.todaysEvents.length) {
          // Sort by date
          // this.todaysEvents = this.todaysEvents.sort((a: Event, b: Event) => {
          //   return a.EventStartDate.localeCompare(b.EventStartDate);
          // });

          // Set current service
          if (this.todaysEvents.find(e => e.IsCurrentEvent)) {
            this.selectedEvent = this.todaysEvents.find(e => e.IsCurrentEvent);
          }

          // if no current service, pick the first one in list
          if (!this.selectedEvent) {
            this.selectedEvent = this.todaysEvents[0];
          }
        }

        // this.subscribeToSignalr();

        this.ready = true;
      },
      error => {
        console.error(error);
        this.ready = true;
      }
    );
  }

  subscribeToSignalr() {
    // Get an observable for events emitted on this channel
    // this.channelService.unsubAll(Constants.CheckinCapacityChannel);
    let channelName = `${Constants.CheckinCapacityChannel}${this.selectedEvent.EventId}${this.kioskDetails.RoomId}`;
    console.log("sub to capacity")
    this.channelService.sub(channelName).subscribe(
      (x: ChannelEvent) => {
        this.room = Room.fromJson(x.Data);
      },
      (error: any) => {
        console.warn('Attempt to join channel failed!', error);
      }
    );
  }

  isActive(event): boolean {
    try {
      return event.EventId === this.selectedEvent.EventId;
    } catch (e) {}
  }

  get selectedEvent(): Event {
    return this.childCheckinService.selectedEvent;
  }

  set selectedEvent(event) {
    this.childCheckinService.selectedEvent = event;
    // make sure to update event room (for capacity purposes)
    this.childCheckinService.getEventRoomDetails(event.EventId, this.kioskDetails.RoomId).subscribe((room) => {
      this.room = room;
      this.subscribeToSignalr();
    }, (error) => this.rootService.announceEvent('generalError'));
  }

  delete(e) {
    this.callNumber = this.callNumber.slice(0, -1);
  }

  clear() {
    this.callNumber = '';
  }

  public getKioskDetails() {
    return this.kioskDetails;
  }

  private resetShowChildModal() {
    this.clear();
    this.overrideChild = new Child();
  }

  setCallNumber(num: string) {
    // set call number
    if (this.callNumber.length < 4) {
      this.callNumber = `${this.callNumber}${num}`;
    }
    // if full call number, search child
    if (this.callNumber.length === 4) {
      this.isOverrideProcessing = true;
      this.childCheckinService.getChildByCallNumber(this.selectedEvent.EventId,
        this.callNumber,
        this.kioskDetails.RoomId).subscribe((child: Child) => {
          this.overrideChild = child;
          this.isOverrideProcessing = false;
      }, (error) => {
        switch (error.status) {
          case 404:
            this.rootService.announceEvent('checkinChildNotFound');
            break;
          default:
            this.rootService.announceEvent('generalError');
            break;
        }
        this.callNumber = '';
        this.isOverrideProcessing = false;
      });
    }
  }

  overrideCheckin() {
    this.isOverrideProcessing = true;
    this.childCheckinService.overrideChildIntoRoom(this.overrideChild, this.selectedEvent.EventId, this.kioskDetails.RoomId)
      .subscribe((child: Child) => {
        this.hideChildSearchModal();
        this.rootService.announceEvent('checkinOverrideSuccess');
        this.isOverrideProcessing = false;
        this.childCheckinService.forceChildReload(this.selectedEvent);
      }, (errorLabel) => {
        switch (errorLabel) {
          case 'capacity':
            this.rootService.announceEvent('checkinOverrideRoomCapacityError');
            break;
          case 'closed':
            this.rootService.announceEvent('checkinOverrideRoomClosedError');
            break;
          default:
            this.rootService.announceEvent('generalError');
            break;
        }
        this.isOverrideProcessing = false;
      });
  }

  public showServiceSelectModal() {
    this.serviceSelectModal.show();
  }

  public hideServiceSelectModal() {
    this.serviceSelectModal.hide();
  }

  public showChildDetailModal() {
    this.childDetailModal.show();
  }

  public showChildSearchModal() {
    this.childSearchModal.show();
  }

  public hideChildSearchModal() {
    this.childSearchModal.hide();
    this.resetShowChildModal();
  }
}
