import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Event } from '../../../shared/models';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';

import * as moment from 'moment';

@Component({
  selector: '.event-import',
  templateUrl: 'event-import.component.html',
  styleUrls: ['event-import.component.scss'],
  providers: [ ],
})
export class EventImportComponent implements OnInit {
  targetEvent: Event;
  events: Event[];
  sourceEventDate: Date;
  sourceEventId: number;
  import: { processing: boolean } = { processing: false };
  ready = false;
  isTemplatePage = false;

  constructor(private route: ActivatedRoute,
    private apiService: ApiService,
    private rootService: RootService,
    private adminService: AdminService,
    private router: Router) {
  }

  ngOnInit() {
    let eventId = this.route.snapshot.params['eventId'];
    this.ready = false;
    if (this.route.snapshot.data['template']) {
      this.isTemplatePage = true;
    }
    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.sourceEventDate = moment(event.EventStartDate).startOf('day').subtract(7, 'days').toDate();
      this.getSourceEventList();
    });
  }

  public getSourceEventList() {
    this.ready = false;
    if (this.isTemplatePage) {
      this.apiService
        .getEventTemplates(this.targetEvent.EventSiteId)
        .subscribe((events: Event[]) => {
          this.sortEvents(events);
          this.ready = true;
        }, (error) => {
          this.ready = true;
        });
    } else {
      this.apiService
        .getEvents(this.sourceEventDate, this.sourceEventDate, this.targetEvent.EventSiteId)
        .subscribe((events: Event[]) => {
          this.sortEvents(events);
          this.ready = true;
        }, (error) => {
          this.ready = true;
        });
    }
  }

  sortEvents(events) {
    this.events = events.filter(e => {
      return e.EventId !== this.targetEvent.EventId;
    }).sort((a: Event, b: Event) => {
      return a.EventStartDate.localeCompare(b.EventStartDate);
    });
  }

  public submitForm(importForm: NgForm): boolean {
    if (importForm.invalid || this.sourceEventId === undefined) {
      this.rootService.announceEvent('echeckImportNoSourceEventSelected');
      return false;
    }

    this.import.processing = true;

    this.adminService.importEvent(this.targetEvent.EventId, this.sourceEventId).subscribe((rooms) => {
      this.rootService.announceEvent('echeckEventImportSuccess');
      this.backToEventRooms();
    }, (error) => {
      this.rootService.announceEvent('echeckEventImportFailure');
      this.import.processing = false;
    });

    return true;
  }

  public backToEventRooms() {
    this.router.navigate([`/admin/events/${this.targetEvent.EventId}/rooms`]);
  }

  public isReady(): boolean {
    return this.ready;
  }
}
