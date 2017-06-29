import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
// import { Event } from '../../../shared/models';
import { ApiService } from '../../../../shared/services';
import { Event } from '../../../../shared/models';
// import { AdminService } from '../../admin.service';

// import * as moment from 'moment';

@Component({
  selector: '.event-import',
  templateUrl: '../event-import.component.html',
  providers: [ ],
})
export class EventImportTemplateComponent implements OnInit {
  ready = false;
  import: { processing: boolean } = { processing: false };
  isTemplatePage = true;
  events: Event[];
  targetEvent: Event;

  constructor(private route: ActivatedRoute,
              private apiService: ApiService) {
  }

  ngOnInit() {
    let eventId = this.route.snapshot.params['eventId'];
    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.apiService
        .getEventTemplates(event.EventSiteId)
        .subscribe((events: Event[]) => {
          // Sort the source events by date & time, and strip off the target event if in the list
          this.events = events.filter(e => {
            return e.EventId !== this.targetEvent.EventId;
          }).sort((a: Event, b: Event) => {
            return a.EventStartDate.localeCompare(b.EventStartDate);
          });
          this.ready = true;
        }, (error) => {
          this.ready = true;
        });
    });
  }

  public isReady() {
    return this.ready;
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
}
