import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Event } from '../../../shared/models';
import { HeaderService } from '../../header/header.service';
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
  import: { processing: boolean, buttonLabel: string } = { processing: false, buttonLabel: 'Import' };

  constructor(private route: ActivatedRoute,
    private apiService: ApiService,
    private headerService: HeaderService,
    private rootService: RootService,
    private adminService: AdminService,
    private router: Router) {
  }

  ngOnInit(): void {
    let eventId = this.route.snapshot.params['eventId'];

    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.headerService.announceEvent(event);
      this.sourceEventDate = moment(event.EventStartDate).startOf('day').subtract(7, 'days').toDate();
      this.changeSourceEventDate(null);
    });
  }

  public changeSourceEventDate($event: any): void {
    this.apiService
      .getEvents(this.sourceEventDate, this.sourceEventDate, this.targetEvent.EventSiteId)
      .subscribe((events: Event[]) => {
        // Sort the source events by date & time, and strip off the target event if in the list
        this.events = events.filter(e => {
          return e.EventId !== this.targetEvent.EventId;
        }).sort((a: Event, b: Event) => {
          return a.EventStartDate.localeCompare(b.EventStartDate);
        });
      });
  }

  public submitForm(importForm: NgForm): boolean {
    if (importForm.invalid || this.sourceEventId === undefined) {
      this.rootService.announceEvent('echeckImportNoSourceEventSelected');
      return false;
    }

    this.import.processing = true;
    this.import.buttonLabel = 'Importing...';

    this.adminService.importEvent(this.targetEvent.EventId, this.sourceEventId).subscribe((rooms) => {
      this.rootService.announceEvent('echeckEventImportSuccess');
      this.backToEventRooms();
    }, (error) => {
      this.rootService.announceEvent('echeckEventImportFailure');
      this.import.processing = false;
      this.import.buttonLabel = 'Import';
    });

    return true;
  }

  public backToEventRooms() {
    this.router.navigate([`/admin/events/${this.targetEvent.EventId}/rooms`]);
  }
}
