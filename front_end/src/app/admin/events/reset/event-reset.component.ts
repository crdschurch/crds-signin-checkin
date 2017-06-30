import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Event } from '../../../shared/models';
import { ApiService, RootService } from '../../../shared/services';
import { AdminService } from '../../admin.service';

@Component({
  selector: '.event-reset',
  templateUrl: 'event-reset.component.html',
  styleUrls: ['event-reset.component.scss'],
  providers: [ ],
})
export class EventResetComponent implements OnInit {
  targetEvent: Event;
  private ready = false;

  constructor(private route: ActivatedRoute,
    private router: Router,
    private rootService: RootService,
    private apiService: ApiService,
    private adminService: AdminService) {
  }

  ngOnInit() {
    let eventId = this.route.snapshot.params['eventId'];
    this.apiService.getEvent(eventId).subscribe((event: Event) => {
      this.targetEvent = event;
      this.ready = true;
    });
  }

  clearEvent() {
    this.ready = false;
    this.adminService.clearEventData(this.targetEvent.EventId).subscribe((event: Event) => {
      this.rootService.announceEvent('checkinEventDataCleared');
      this.router.navigate(['/admin/events', this.targetEvent.EventId, 'rooms']);
    }, (err) => {
      this.ready = true;
      console.error(err);
      this.rootService.announceEvent('generalError');
    });
  }

  isReady() {
    return this.ready;
  }
}
